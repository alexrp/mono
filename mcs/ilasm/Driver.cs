//
// Mono.ILAsm.Driver
//    Main Command line interface for Mono ILAsm Compiler
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//
// (C) 2003 Jackson Harper, All rights reserved
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Mono.Security;

namespace Mono.ILAsm
{
	public sealed class Driver
	{
		private enum Target : byte
		{
			Dll,
			Exe
		}

		public static int Main (string[] args)
		{
			// Do everything in Invariant
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			var driver = new DriverMain (args);
			if (!driver.Run ()) {
				return 1;
			}

			Report.Message ("Operation completed successfully");
			return 0;
		}

		private sealed class DriverMain
		{
			private List<string> il_file_list;
			private string output_file;
			private Target target = Target.Exe;
			private string target_string;
			private bool show_tokens;
			//private bool show_method_def;
			//private bool show_method_ref;
			private bool show_parser;
			private bool scan_only;
			private bool debugging_info;
			private CodeGen codegen;
			private bool key_container;
			private string key_name;
			private StrongName sn;


			public DriverMain (string[] args)
			{
				il_file_list = new List<string> ();
				ParseArgs (args);
			}


			public bool Run ()
			{
				if (il_file_list.Count == 0) {
					Usage ();
				}

				if (output_file == null) {
					output_file = CreateOutputFileName ();
				}

				try {
					codegen = new CodeGen (output_file, target == Target.Dll, debugging_info);
					foreach (var file_path in il_file_list) {
						Report.FilePath = file_path;
						ProcessFile (file_path);
					}
					
					if (scan_only) {
						return true;
					}

					if (Report.ErrorCount > 0) {
						return false;
					}

					if (target != Target.Dll && !codegen.HasEntryPoint) {
						Report.Error ("No entry point found.");
					}

					// if we have a key and aren't assembling a netmodule
					if ((key_name != null) && !codegen.IsThisAssembly (null)) {
						LoadKey ();
						// this overrides any attribute or .publickey directive in the source
						codegen.ThisAssembly.SetPublicKey (sn.PublicKey);
					}

					try {
						codegen.Write ();
					} catch {
						File.Delete (output_file);
						throw;
					}
				} catch (ILAsmException e) {
					Error (e.ToString ());
					return false;
				} catch (Exception ex) {
					Error ("Error: " + ex.Message);
					return false;
				} 

				try {
					if (sn != null) {
						Report.Message ("Signing assembly with the specified strong name key pair");
						return Sign (output_file);
					}
				} catch {
					return false;
				}

				return true;
			}


			private void Error (string message)
			{
				Console.WriteLine (message + Environment.NewLine);
				Console.WriteLine ("***** FAILURE *****" + Environment.NewLine);
			}


			private void LoadKey ()
			{
				if (key_container) {
					CspParameters csp = new CspParameters ();
					csp.KeyContainerName = key_name;
					RSACryptoServiceProvider rsa = new RSACryptoServiceProvider (csp);
					sn = new StrongName (rsa);
				} else {
					byte[] data = null;
					using (FileStream fs = File.OpenRead (key_name)) {
						data = new byte [fs.Length];
						fs.Read (data, 0, data.Length);
						fs.Close ();
					}
					sn = new StrongName (data);
				}
			}


			private bool Sign (string fileName)
			{
				// note: if the file cannot be signed (no public key in it) then
				// we do not show an error, or a warning, if the key file doesn't
				// exist
				return sn.Sign (fileName);
			}


			private void ProcessFile (string filePath)
			{
				if (!File.Exists (filePath)) {
					Console.WriteLine ("File does not exist: {0}", filePath);
					Environment.Exit (2);
				}
				
				Report.AssembleFile (filePath, null, target_string, output_file);
				var reader = File.OpenText (filePath);
				var scanner = new ILTokenizer (reader);

				if (show_tokens) {
					scanner.NewTokenEvent += new NewTokenEvent (ShowToken);
				}

				//if (show_method_def)
				//        MethodTable.MethodDefinedEvent += new MethodDefinedEvent (ShowMethodDef);
				//if (show_method_ref)
				//       MethodTable.MethodReferencedEvent += new MethodReferencedEvent (ShowMethodRef);

				if (scan_only) {
					ILToken tok;
					while ((tok = scanner.NextToken) != ILToken.EOF) {
						Console.WriteLine (tok);
					}
					return;
				}

				ILParser parser = new ILParser (codegen, scanner);
				codegen.BeginSourceFile (filePath);
				try {
					if (show_parser) {
						parser.yyparse (new ScannerAdapter (scanner), new yydebug.yyDebugSimple ());
					} else {
						parser.yyparse (new ScannerAdapter (scanner), null);
					}
				} catch (ILTokenizingException ilte) {
					Report.Error (ilte.Location, "syntax error at token '" + ilte.Token + "'");
				} catch (Mono.ILASM.yyParser.yyException ye) {
					Report.Error (scanner.Reader.Location, ye.Message);
				} catch (ILAsmException ie) {
					ie.FilePath = filePath;
					ie.Location = scanner.Reader.Location;
					throw;
				} catch (Exception) {
					Console.Write ("{0} ({1}, {2}): ", filePath, scanner.Reader.Location.line, scanner.Reader.Location.column);
					throw;
				} finally {
					codegen.EndSourceFile ();
				}
			}


			public void ShowToken (object sender, NewTokenEventArgs args)
			{
				Console.WriteLine ("token: '{0}'", args.Token);
			}
		
			/*
			public void ShowMethodDef (object sender, MethodDefinedEventArgs args)
			{
				Console.WriteLine ("***** Method defined *****");
				Console.WriteLine ("-- signature:   {0}", args.Signature);
				Console.WriteLine ("-- name:        {0}", args.Name);
				Console.WriteLine ("-- return type: {0}", args.ReturnType);
				Console.WriteLine ("-- is in table: {0}", args.IsInTable);
				Console.WriteLine ("-- method atts: {0}", args.MethodAttributes);
				Console.WriteLine ("-- impl atts:   {0}", args.ImplAttributes);
				Console.WriteLine ("-- call conv:   {0}", args.CallConv);
			}

			public void ShowMethodRef (object sender, MethodReferencedEventArgs args)
			{
				Console.WriteLine ("***** Method referenced *****");
				Console.WriteLine ("-- signature:   {0}", args.Signature);
				Console.WriteLine ("-- name:        {0}", args.Name);
				Console.WriteLine ("-- return type: {0}", args.ReturnType);
				Console.WriteLine ("-- is in table: {0}", args.IsInTable);
			}
			*/
		
			private void ParseArgs (string[] args)
			{
				string command_arg;
				foreach (var str in args) {
					if ((str [0] != '-') && (str [0] != '/')) {
						il_file_list.Add (str);
						continue;
					}
					
					switch (GetCommand (str, out command_arg)) {
					case "out":
					case "output":
						output_file = command_arg;
						break;
					case "exe":
						target = Target.Exe;
						target_string = "exe";
						break;
					case "dll":
						target = Target.Dll;
						target_string = "dll";
						break;
					case "quiet":
						Report.Quiet = true;
						break;
					case "debug":
					case "deb":
						debugging_info = true;
						break;
					// Stubs to stay commandline compatible with MS
					case "listing":
					case "nologo":
					case "clock":
					case "error":
					case "subsystem":
					case "flags":
					case "alignment":
					case "base":
					case "resource":
						break;
					case "key":
						if (command_arg.Length > 0) {
							key_container = (command_arg [0] == '@');
						}

						if (key_container) {
							key_name = command_arg.Substring (1);
						} else {
							key_name = command_arg;
						}

						break;
					case "scan_only":
						scan_only = true;
						break;
					case "show_tokens":
						show_tokens = true;
						break;
					//case "show_method_def":
					//        show_method_def = true;
					//        break;
					//case "show_method_ref":
					//        show_method_ref = true;
					//        break;
					case "show_parser":
						show_parser = true;
						break;
					case "-about":
						if (str [0] != '-') {
							break;
						}
						
						About ();
						break;
					case "-version":
						if (str [0] != '-') {
							break;
						}

						Version ();
						break;
					default:
						if (str [0] == '-') {
							break;
						}

						il_file_list.Add (str);
						break;
					}
				}
			}


			private string GetCommand (string str, out string command_arg)
			{
				int end_index = str.IndexOfAny (new char[] {':', '='}, 1);
				var command = str.Substring (1, end_index == -1 ? str.Length - 1 : end_index - 1);

				if (end_index != -1) {
					command_arg = str.Substring (end_index + 1);
				} else {
					command_arg = null;
				}

				return command.ToLower ();
			}

			/// <summary>
			/// Get the first file name and makes it into an output file name
			/// </summary>
			private string CreateOutputFileName ()
			{
				var file_name = (string)il_file_list [0];
				int ext_index = file_name.LastIndexOf ('.');

				if (ext_index == -1) {
					ext_index = file_name.Length;
				}

				return String.Format ("{0}.{1}", file_name.Substring (0, ext_index), target_string);
			}


			private void Usage ()
			{
				Console.WriteLine ("Mono ILAsm compiler{0}" +
					"ilasm [options] <source files>{0}" +
					"   --about            About the Mono ILAsm compiler.{0}" +
					"   --version          Print the version number of the Mono ILAsm compiler.{0}" +
					"   /output:file_name  Specifies output file.{0}" +
					"   /exe               Compile to executable.{0}" +
					"   /dll               Compile to library.{0}" +
					"   /debug             Include debug information.{0}" +
					"   /key:keyfile       Strong name using the specified key file.{0}" +
					"   /key:@container    Strong name using the specified key container.{0}" +
					"Options can be of the form -option or /option{0}", Environment.NewLine);
				Environment.Exit (1);
			}


			private void About ()
			{
				Console.WriteLine ("For more information on Mono, visit the project Web site{0}" +
					"   http://www.go-mono.com{0}{0}", Environment.NewLine);
				Environment.Exit (0);
			}


			private void Version ()
			{
				string version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
				Console.WriteLine ("Mono ILAsm compiler version {0}", version);
				Environment.Exit (0);
			}
		}
	}
}
