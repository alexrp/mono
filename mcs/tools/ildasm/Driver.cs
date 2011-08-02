//
// Mono.ILDasm.Driver
//
// Author(s):
//  Alex Rønne Petersen <alex@alexrp.com>
//
// Copyright (C) Alex Rønne Petersen
//
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Mdb;

namespace Mono.ILDasm {
	public sealed class Driver {
		string target_file;
		Output output_type = Output.Console; // TODO: Add a GUI?
		string output_file;
		Encoding output_encoding = Encoding.ASCII;
		bool ca_verbal;
		bool no_ca;
		bool raw_bytes;
		bool quote_all;
		bool raw_eh;
		bool show_md_tokens;
		Visibility? visibility;
		bool no_il;
		
		public ExitCode? Run (string[] args)
		{
			if (!ParseArgs (args))
				return null;
			
			try {
				var hasSymbols = File.Exists (target_file + ".mdb");
				
				var module = ModuleDefinition.ReadModule (target_file, new ReaderParameters {
					SymbolReaderProvider = hasSymbols ?
						new MdbReaderProvider () : null,
				});
				
				TextWriter output = null;
				
				switch (output_type)
				{
				case Output.Console:
					output = Console.Out;
					break;
				case Output.File:
					output = new StreamWriter (output_file,
						false, output_encoding);
					break;
				case Output.RichText:
				case Output.Html:
				case Output.Gui:
					throw new Exception ("Output type not supported.");
				}
				
				new ModuleDisassembler (output, module) {
					VerbalCustomAttributes = ca_verbal,
					NoCustomAttributes = no_ca,
					RawBytes = raw_bytes,
					QuoteAll = quote_all,
					RawExceptionHandlers = raw_eh,
					ShowMetadataTokens = show_md_tokens,
					Visibility = visibility,
					NoCil = no_il,
				}.Disassemble ();
			} catch (Exception ex) {
				Logger.Error (ex.ToString ());
				return ExitCode.Error;
			}

			return ExitCode.Success;
		}

		bool ParseArgs (string[] args)
		{
			string commandArg;
			
			foreach (var str in args) {
				if (str [0] != '-' && str [0] != '/') {
					target_file = str;
					continue;
				}
				
				var cmd = GetCommand (str, out commandArg);
				
				switch (cmd) {
				case "out":
				case "outp":
				case "outpu":
				case "output":
					output_type = Output.File;
					output_file = commandArg;
					break;
				case "rtf":
					output_type = Output.RichText;
					throw new NotImplementedException ();
				case "htm":
				case "html":
					output_type = Output.Html;
					throw new NotImplementedException ();
				case "tex":
				case "text":
					output_type = Output.Console;
					break;
				case "byt":
				case "byte":
				case "bytes":
					raw_bytes = true;
					break;
				case "cav":
				case "cave":
				case "caver":
				case "caverb":
				case "caverba":
				case "caverbal":
					ca_verbal = true;
					break;
				case "lin":
				case "line":
				case "linen":
				case "linenu":
				case "linenum":
					throw new NotImplementedException ();
				case "nob":
				case "noba":
				case "nobar":
					// We don't have a progress bar.
					break;
				case "noc":
				case "noca":
					no_ca = true;
					break;
				case "pub":
				case "pubo":
				case "pubon":
				case "pubonl":
				case "pubonly":
					visibility = Visibility.Public;
					break;
				case "quo":
				case "quot":
				case "quote":
				case "quotea":
				case "quoteal":
				case "quoteall":
				case "quotealln":
				case "quoteallna":
				case "quoteallnam":
				case "quoteallname":
				case "quoteallnames":
					quote_all = true;
					break;
				case "raw":
				case "rawe":
				case "raweh":
					raw_eh = true;
					break;
				case "sou":
				case "sour":
				case "sourc":
				case "source":
					throw new NotImplementedException ();
				case "tok":
				case "toke":
				case "token":
				case "tokens":
					show_md_tokens = true;
					break;
				case "vis":
				case "visi":
				case "visib":
				case "visibi":
				case "visibil":
				case "visibili":
				case "visibilit":
				case "visibility":
					switch (commandArg.ToLower ())
					{
					case "pub":
						visibility = Visibility.Public;
						break;
					case "pri":
						visibility = Visibility.Private;
						break;
					case "fam":
						visibility = Visibility.Family;
						break;
					case "asm":
						visibility = Visibility.Assembly;
						break;
					case "faa":
						visibility = Visibility.FamANDAssem;
						break;
					case "foa":
						visibility = Visibility.FamORAssem;
						break;
					case "psc":
						visibility = Visibility.PrivateScope;
						break;
					default:
						Logger.Fatal ("'{0}' is not a valid visibility",
							commandArg);
						break;
					}
					break;
				case "all":
				case "cla":
				case "clas":
				case "class":
				case "classl":
				case "classli":
				case "classlis":
				case "classlist":
					throw new NotImplementedException ();
				case "for":
				case "forw":
				case "forwa":
				case "forwar":
				case "forward":
					throw new NotImplementedException ();
				case "hea":
				case "head":
				case "heade":
				case "header":
					throw new NotImplementedException ();
				case "ite":
				case "item": // = class[::member[(sig]]
					throw new NotImplementedException ();
				case "noi":
				case "noil":
					no_il = true;
					break;
				case "sta":
				case "stat":
				case "stats":
					throw new NotImplementedException ();
				case "typ":
				case "type":
				case "typel":
				case "typeli":
				case "typelis":
				case "typelist":
					throw new NotImplementedException ();
				case "uni":
				case "unic":
				case "unico":
				case "unicod":
				case "unicode":
					output_encoding = Encoding.Unicode;
					break;
				case "utf":
				case "utf8":
					output_encoding = Encoding.UTF8;
					break;
				case "nom":
				case "nome":
				case "nomet":
				case "nometa":
				case "nometad":
				case "nometada":
				case "nometadat":
				case "nometadata": // = mdheader | hex | csv | unrex | schema | raw | heaps | validate
					throw new NotImplementedException ();
				case "obj":
				case "obje":
				case "objec":
				case "object":
				case "objectf":
				case "objectfi":
				case "objectfil":
				case "objectfile":
					throw new NotImplementedException ();
				case "?":
					Usage ();
					return false;
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
				default:
					if (str [0] == '-' || str [0] == '/')
						break;

					target_file = str;
					break;
				}
			}
			
			return true;
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

		void Usage ()
		{
			var n = Environment.NewLine;
			
			Console.WriteLine ("Mono IL Disassembler{0}" +
				"ildasm [options] <target file>{0}" +
				"   --about             About the Mono IL disassembler.{0}" +
				"   --version           Print the version number of the Mono IL disassembler.{0}" +
				"Options can be of the form -option or /option.{0}",
				n);
		}

		void About ()
		{
			Console.WriteLine ("For more information on Mono, visit the project Web site{0}" +
				"   http://www.go-mono.com", Environment.NewLine);
		}

		void Version ()
		{
			var version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			Console.WriteLine ("Mono ILDasm version {0}", version);
		}
	}
}
