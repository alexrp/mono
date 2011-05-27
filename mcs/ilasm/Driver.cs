//
// Mono.ILAsm.Driver
//    Main Command line interface for Mono ILAsm Compiler
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//  Alex Rønne Petersen <alex@alexrp.com>
//
// (C) 2003 Jackson Harper, All rights reserved
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Mono.Security;

namespace Mono.ILAsm {
	public sealed class DriverMain {
		private readonly List<string> il_file_list = new List<string> ();
		private string output_file;
		private Target target;
		private bool show_tokens;
		private bool show_parser;
		private bool scan_only;
		private bool debugging_info;
		private bool key_container;
		private string key_name;

		public DriverMain (string[] args)
		{
			ParseArgs (args);
		}

		public bool Run ()
		{
			if (il_file_list.Count == 0)
				Usage (false, true);

			if (output_file == null)
				output_file = CreateOutputFileName (il_file_list, target);
			
			var codegen = new CodeGenerator (output_file,
				target == Target.Dll, debugging_info);
			StrongName sn = null;
			
			try {
				foreach (var file_path in il_file_list) {
					Report.FilePath = file_path;
					ProcessFile (codegen, file_path);
				}
				
				if (scan_only)
					return true;
				if (target != Target.Dll && !codegen.HasEntryPoint)
					Report.Error ("No entry point found.");
				
				if (key_name != null) {
					sn = LoadKey (key_container, key_name);
					// this overrides any attribute or .publickey directive in the source
					codegen.CurrentModule.Assembly.Name.PublicKey =
						sn.PublicKey;
				}

				try {
					codegen.Write (output_file);
				} catch (Exception) {
					File.Delete (output_file);
					throw;
				}
			} catch (Exception ex) {
				Error ("{0}{1}{2}", ex.ToString (), Environment.NewLine,
					ex.StackTrace);
				return false;
			}
			
			if (sn != null)
				try {
					Report.Message ("Signing assembly with the specified strong name key pair...");
					Sign (sn, output_file);
				} catch (Exception ex) {
					Error ("Could not sign assembly: {0}", ex.Message);
					return false;
				}

			return true;
		}

		private void ProcessFile (CodeGenerator codegen, string filePath)
		{
			if (!File.Exists (filePath)) {
				Report.Error ("File does not exist: {0}", filePath);
				Environment.Exit (2);
			}
			
			Report.AssembleFile (filePath, target.ToString ().ToUpper (), output_file);
			var reader = File.OpenText (filePath);
			var scanner = new ILTokenizer (reader);

			if (show_tokens)
				scanner.NewTokenEvent += new NewTokenEvent (ShowToken);

			if (scan_only) {
				ILToken tok;
				while ((tok = scanner.NextToken) != ILToken.EOF)
					Console.WriteLine (tok);
				return;
			}

			var parser = new ILParser (codegen, scanner);
			try {
				parser.yyparse (new ScannerAdapter (scanner),
					show_parser ? new yydebug.yyDebugSimple () : null);
			} catch (ILTokenizingException ilte) {
				Report.Error (ilte.Location, "Syntax error at token '" + ilte.Token + "'");
			} catch (yyParser.yyException ye) {
				Report.Error (scanner.Reader.Location, ye.Message);
			} catch (ILAsmException) {
				throw;
			} catch (Exception ex) {
				throw new ILAsmException (ex.Message, scanner.Reader.Location, filePath, ex);
			}
		}

		private void ParseArgs (string[] args)
		{
			string command_arg;
			foreach (var str in args) {
				if ((str [0] != '-') && (str [0] != '/')) {
					il_file_list.Add (str);
					continue;
				}
				
				var cmd = GetCommand (str, out command_arg);
				switch (cmd) {
				case "out":
				case "output":	
					output_file = command_arg;
					break;
				case "exe":
					target = Target.Exe;
					break;
				case "dll":
					target = Target.Dll;
					break;
				case "quiet":
					Report.Quiet = true;
					break;
				case "debug":
				case "deb":
					// TODO: support impl and opt
					debugging_info = true;
					break;
				// Stubs to stay commandline compatible with MS
				case "nologo":
					break;
				case "noautoinherit":
				case "nocorstub":
				case "stripreloc":
				case "clock":
				case "error":
				case "subsystem":
				case "flags":
				case "alignment":
				case "base":
				case "resource":
				case "pdb":
				case "optimize":
				case "fold":
				case "include":
				case "stack":
				case "enc":
				case "mdv":
				case "msv":
				case "itanium":
				case "x64":
				case "pe64":
					Report.Warning ("Unimplemented command line option: {0}", cmd);
					break;
				case "key":
					if (command_arg.Length > 0)
						key_container = (command_arg [0] == '@');

					if (key_container)
						key_name = command_arg.Substring (1);
					else
						key_name = command_arg;

					break;
				case "?":
					Usage (false, false);
					break;
				case "mono_?":
					Usage (true, false);
					break;
				case "mono_scanonly":
					scan_only = true;
					break;
				case "mono_showtokens":
					show_tokens = true;
					break;
				case "mono_showparser":
					show_parser = true;
					break;
				case "-about":
					if (str [0] != '-')
						break;
					
					About ();
					break;
				case "-version":
					if (str [0] != '-')
						break;

					Version ();
					break;
				default:
					if (str [0] == '-')
						break;

					il_file_list.Add (str);
					break;
				}
			}
		}
		
		private static string CreateOutputFileName (IList<string> ilFileList, Target target)
		{
			var file_name = ilFileList [0];
			var ext_index = file_name.LastIndexOf ('.');

			if (ext_index == -1)
				ext_index = file_name.Length;

			return file_name.Substring (0, ext_index) +
				(target == Target.Dll ? ".dll" : ".exe");
		}

		private static void Error (string message, params object[] args)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine (string.Format (message, args));
			Console.Error.WriteLine ("***** FAILURE *****");
			Console.ResetColor ();
		}

		private static StrongName LoadKey (bool keyContainer, string keyName)
		{
			StrongName sn;
			
			if (keyContainer) {
				var csp = new CspParameters ();
				csp.KeyContainerName = keyName;
				var rsa = new RSACryptoServiceProvider (csp);
				
				sn = new StrongName (rsa);
			} else {
				byte[] data = null;
				using (var fs = File.OpenRead (keyName)) {
					data = new byte [fs.Length];
					fs.Read (data, 0, data.Length);
					fs.Close ();
				}
				
				sn = new StrongName (data);
			}
			
			return sn;
		}

		private static bool Sign (StrongName sn, string fileName)
		{
			// note: if the file cannot be signed (no public key in it) then
			// we do not show an error, or a warning, if the key file doesn't
			// exist
			return sn.Sign (fileName);
		}

		public static void ShowToken (object sender, NewTokenEventArgs args)
		{
			Console.WriteLine ("Token: {0}", args.Token);
		}

		private static string GetCommand (string str, out string command_arg)
		{
			var end_index = str.IndexOfAny (new char[] { ':', '=' }, 1);
			var command = str.Substring (1, (end_index == -1 ? str.Length : end_index) - 1);

			if (end_index != -1)
				command_arg = str.Substring (end_index + 1);
			else
				command_arg = null;

			return command.ToLower ();
		}

		private static void Usage (bool dev, bool error)
		{
			var n = Environment.NewLine;
			
			Console.WriteLine ("Mono IL Assembler{0}" +
				"ilasm [options] <source files>{0}" +
				"   --about             About the Mono ILAsm compiler.{0}" +
				"   --version           Print the version number of the Mono ILAsm compiler.{0}" +
				"   /output:file_name   Specifies output file.{0}" +
				"   /exe                Compile to executable.{0}" +
				"   /dll                Compile to library.{0}" +
				"   /debug              Include debug information.{0}" +
				"   /key:key_file       Strong name using the specified key file.{0}" +
				"   /key:@key_container Strong name using the specified key container.{0}" +
				"Options can be of the form -option or /option.{0}",
				n);
			
			if (!dev)
				Console.WriteLine ("Pass /mono_? for developer options.", n);
			else
				Console.WriteLine ("Developer options:{0}" +
					"   /mono_scanonly      Only perform tokenization.{0}" +
					"   /mono_showtokens    Show tokens as they're scanned.{0}" +
					"   /mono_showparser    Show parser debug output.{0}",
					n);
			
			Environment.Exit (error ? 1 : 0);
		}

		private static void About ()
		{
			Console.WriteLine ("For more information on Mono, visit the project Web site{0}" +
				"   http://www.go-mono.com{0}", Environment.NewLine);
			
			Environment.Exit (0);
		}

		private static void Version ()
		{
			var version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			Console.WriteLine ("Mono IL Assembler version {0}", version);
			
			Environment.Exit (0);
		}
	}
}
