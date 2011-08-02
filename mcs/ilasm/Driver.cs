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
	public sealed class Driver {
		readonly List<string> il_file_list = new List<string> ();
		bool show_tokens;
		bool show_parser;
		bool scan_only;
		bool key_container;
		bool no_auto_inherit;
		string key_name;
		ILTokenizer scanner;
		
		public string OutputFileName { get; set; }
		
		public Target Target { get; set; }
		
		public bool DebuggingInfo { get; set; }
		
		public TextWriter Output { get; set; }
		
		public Report Report { get; private set; }
		
		public Driver ()
		{
			Report = new Report ();
			Output = Console.Out;
		}

		public ExitCode? Run (string[] args)
		{
			if (!ParseArgs (args))
				return null;
			
			if (il_file_list.Count == 0) {
				Usage (false);
				return ExitCode.Error;
			}

			if (OutputFileName == null)
				OutputFileName = CreateOutputFileName (il_file_list, Target);
			
			var codegen = new CodeGenerator (Report, OutputFileName, Target) {
				DebuggingSymbols = DebuggingInfo,
				NoAutoInherit = no_auto_inherit,
			};
			
			StrongName sn = null;
			
			try {
				foreach (var file_path in il_file_list) {
					var fileResult = ProcessFile (codegen, file_path);
					
					if (fileResult != ExitCode.Success)
						return fileResult;
				}
				
				if (scan_only)
					return 0;
				
				if (Target != Target.Dll && codegen.CurrentModule.EntryPoint == null)
					Report.WriteError (Error.NoEntryPoint, "No entry point found.");
				
				if (key_name != null) {
					sn = LoadKey (key_container, key_name);
					// This overrides any attribute or .publickey directive in the source.
					codegen.CurrentModule.Assembly.Name.PublicKey = sn.PublicKey;
				}
				
				codegen.Write (OutputFileName);
				
				if (sn != null)
					try {
						Report.WriteMessage ("Signing assembly with the specified strong name key pair...");
						Sign (sn, OutputFileName);
					} catch (Exception ex) {
						Report.WriteError (Error.SigningFailed, "Could not sign assembly: {0}",
							ex.Message);
					}
			} catch (ILAsmException ie) {
				WriteError (ie.ToString ());
				return ExitCode.Error;
			} catch (Exception ex) {
				// An internal error has occurred...
				WriteError ("{0}", ex.ToString ());
				WriteError ("{0}", ex.StackTrace);
				
				if (scanner != null)
					WriteError ("Location: {0}", scanner.Reader.Location);
				
				WriteError ("This is an internal error. Please file a bug report.");
				WriteFailure ();
				
				return ExitCode.Error;
			}

			return ExitCode.Success;
		}

		ExitCode ProcessFile (CodeGenerator codegen, string filePath)
		{
			if (!File.Exists (filePath)) {
				Report.WriteError (Error.FileNotFound, "File does not exist: {0}", filePath);
				return ExitCode.Abort;
			}
			
			Report.WriteMessage ("Assembling {0} to {1} -> {2}...",
				filePath, Target.ToString ().ToUpper (), OutputFileName);
			Report.WriteMessage (string.Empty);
			
			var reader = File.OpenText (filePath);
			scanner = new ILTokenizer (reader);
			
			Report.FilePath = filePath;
			Report.Tokenizer = scanner;

			if (show_tokens)
				scanner.NewToken += ShowToken;

			if (scan_only) {
				ILToken tok;
				while ((tok = scanner.GetNextToken ()) != ILToken.EOF)
					Console.WriteLine (tok);
				return ExitCode.Success;
			}

			var parser = new ILParser (Report, codegen, scanner);
			try {
				parser.yyparse (new ScannerAdapter (scanner),
					show_parser ? new yydebug.yyDebugSimple () : null);
			} catch (ILTokenizingException ilte) {
				Report.WriteError (Error.SyntaxError, ilte.Location, "Syntax error at token '" + ilte.Token + "'.");
			} catch (yyParser.yyException ye) {
				Report.WriteError (Error.SyntaxError, scanner.Reader.Location, "Syntax error: " + ye.Message);
			} finally {
				Report.Tokenizer = null;
				Report.FilePath = null;
			}
			
			return ExitCode.Success;
		}

		bool ParseArgs (string[] args)
		{
			string commandArg;
			foreach (var str in args) {
				if ((str [0] != '-') && (str [0] != '/')) {
					il_file_list.Add (str);
					continue;
				}
				
				var cmd = GetCommand (str, out commandArg);
				switch (cmd) {
				case "out":
				case "outp":
				case "outpu":
				case "output":	
					OutputFileName = commandArg;
					break;
				case "exe":
					Target = Target.Exe;
					break;
				case "dll":
					Target = Target.Dll;
					break;
				case "qui":
				case "quie":
				case "quiet":
					Report.Quiet = true;
					break;
				case "deb":
				case "debu":
				case "debug":
				case "pdb":
					// TODO: Support impl and opt (add DebuggableAttribute
					// with the 1 | 2 for opt, 1 | 2 | 256 for opt).
					DebuggingInfo = true;
					break;
				case "nol":
				case "nolo":
				case "nolog":
				case "nologo":
					break; // We don't print a logo...
				case "noa":
				case "noau":
				case "noaut":
				case "noauto":
				case "noautoi":
				case "noautoin":
				case "noautoinh":
				case "noautoinhe":
				case "noautoinher":
				case "noautoinheri":
				case "noautoinherit":
					no_auto_inherit = true;
					break;
				// Stubs to stay command line compatible with MS.
				case "noc": // TODO: How can we do these with Cecil?
				case "noco":
				case "nocor":
				case "nocors":
				case "nocorst":
				case "nocorstu":
				case "nocorstub":
				case "str":
				case "stri":
				case "strip":
				case "stripr":
				case "stripre":
				case "striprel":
				case "striprelo":
				case "stripreloc":
					break;
				case "fla":
				case "flag":
				case "flags":
				case "ali":
				case "alig":
				case "align":
				case "alignm":
				case "alignme":
				case "alignmen":
				case "alignment":
				case "bas":
				case "base":
				case "sta":
				case "stac":
				case "stack":
				case "mdv":
				case "msv":
					// TODO: These cannot be implemented until
					// we can implement the corresponding language
					// elements.
					break;
				case "res": // TODO: How do we include in *.res format?
				case "reso":
				case "resou":
				case "resour":
				case "resourc":
				case "resource":
				case "clo":
				case "cloc":
				case "clock": // TODO: Do timings for each assembly step.
				case "sub":
				case "subs":
				case "subsy":
				case "subsys":
				case "subsyst":
				case "subsyste":
				case "subsystem":
				case "opt": // TODO: Bring in Mono.Cecil.Rocks.
				case "opti":
				case "optim":
				case "optimi":
				case "optimiz":
				case "optimize":
				case "fol":
				case "fold":
				case "inc":
				case "incl":
				case "inclu":
				case "includ":
				case "include": // TODO: Actually implement the preprocessor.
				case "itanium":
				case "x64":
				case "pe6":
				case "pe64":
					Report.WriteWarning (Warning.InternalWarning,
						"Unimplemented command line option: {0}", cmd);
					break;
				case "enc":
					Report.WriteWarning (Warning.InternalWarning,
						"Unsupported command line option: {0}", cmd);
					break;
				case "key":
					if (commandArg.Length > 0)
						key_container = (commandArg [0] == '@');

					if (key_container)
						key_name = commandArg.Substring (1);
					else
						key_name = commandArg;

					break;
				case "?":
					Usage (false);
					return false;
				case "mono_?":
					Usage (true);
					return false;
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
					return false;
				case "-version":
					if (str [0] != '-')
						break;

					Version ();
					return false;
				case "v":
				case "-verbose":
					Report.Verbose = true;
					break;
				default:
					if (str [0] == '-')
						break;

					il_file_list.Add (str);
					break;
				}
			}
			
			return true;
		}
		
		static string CreateOutputFileName (IList<string> ilFileList, Target target)
		{
			var fileName = ilFileList [0];
			var extIndex = fileName.LastIndexOf ('.');

			if (extIndex == -1)
				extIndex = fileName.Length;

			return fileName.Substring (0, extIndex) +
				(target == Target.Dll ? ".dll" : ".exe");
		}

		void WriteError (string message, params object[] args)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Report.ErrorOutput.WriteLine (string.Format (message, args));
			Console.ResetColor ();
		}
		
		void WriteFailure ()
		{
			WriteError ("***** FAILURE *****");
		}

		static StrongName LoadKey (bool keyContainer, string keyName)
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

		static bool Sign (StrongName sn, string fileName)
		{
			// Note: If the file cannot be signed (no public key in it) then
			// we do not show an error or a warning, if the key file doesn't
			// exist.
			return sn.Sign (fileName);
		}

		static void ShowToken (object sender, NewTokenEventArgs args)
		{
			Console.WriteLine ("Token: {0}", args.Token);
		}

		static string GetCommand (string str, out string commandArg)
		{
			var endIndex = str.IndexOfAny (new[] { ':', '=' }, 1);
			var command = str.Substring (1, (endIndex == -1 ? str.Length : endIndex) - 1);

			if (endIndex != -1)
				commandArg = str.Substring (endIndex + 1);
			else
				commandArg = null;

			return command.ToLower ();
		}

		void Usage (bool dev)
		{
			var n = Environment.NewLine;
			
			Output.WriteLine ("Mono IL Assembler{0}" +
				"ilasm [options] <source files>{0}" +
				"   --about             About the Mono IL assembler.{0}" +
				"   --version           Print the version number of the Mono IL assembler.{0}" +
				"   /verbose            Be verbose.{0}" +
				"   /quiet              Don't print plain messages.{0}" +
				"   /nologo             Don't print a logo.{0}" +
				"   /output:file_name   Specifies output file.{0}" +
				"   /exe                Compile to executable.{0}" +
				"   /dll                Compile to library.{0}" +
				"   /debug[=impl|opt]   Include debug information.{0}" +
				"   /key:key_file       Strong name using the specified key file.{0}" +
				"   /key:@key_container Strong name using the specified key container.{0}" +
				"   /noautoinherit      Don't automatically inherit from System.Object.{0}" +
				"   /flags=int          Set module flags.{0}" +
				"   /alignment=int      Set file alignment.{0}" +
				"   /base=int           Set module image base.{0}" +
				"   /stack=int          Set the stack reserve.{0}" +
				"   /mdv=versionString  Set metadata version string.{0}" +
				"   /msv=major.minor    Set metadata stream version (integers).{0}" +
				"   /nocorstub          Don't generate CORExeMain stub.{0}" +
				"   /stripreloc         No base relocations needed.{0}" +
				"Options can be of the form -option or /option.{0}",
				n);
			
			if (!dev)
				Output.WriteLine ("Pass /mono_? for developer options.", n);
			else
				Output.WriteLine ("Developer options:{0}" +
					"   /mono_scanonly      Only perform tokenization.{0}" +
					"   /mono_showtokens    Show tokens as they're scanned.{0}" +
					"   /mono_showparser    Show parser debug output.",
					n);
		}

		void About ()
		{
			Output.WriteLine ("For more information on Mono, visit the project Web site{0}" +
				"   http://www.go-mono.com", Environment.NewLine);
		}

		void Version ()
		{
			var version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			Output.WriteLine ("Mono ILAsm version {0}", version);
		}
	}
}
