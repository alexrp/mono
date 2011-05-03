using System;
using System.IO;

using Mono.ILASM;

namespace Mono.ILAsm {
	public static class ILAsmTest {
		public static int Main (string[] args)
		{
			if (args.Length != 1) {
				Console.WriteLine ("Usage: ilasm [filename]");
				return 1;
			}

			var reader = File.OpenText (args [0]);
			var scanner = new ILTokenizer (reader);

			const bool testScanner = true;

			if (testScanner) {
				ILToken tok;
				while ((tok = scanner.NextToken) != ILToken.EOF) {
					Console.WriteLine (tok);
				}
			} else {
				var parser = new ILParser (new CodeGen ());
				parser.yyparse (new ScannerAdapter (scanner), new yydebug.yyDebugSimple ());

				var cg = parser.CodeGen;
				int n = cg.ClassCount;
				cg.Emit ();
			}

			return 0;
		}
	}
}
