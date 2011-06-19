// StringHelper.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.Text;

namespace Mono.ILAsm {
	internal sealed class StringHelper : StringHelperBase {
		public StringHelper (ILTokenizer host)
			: base (host)
		{
		}

		public override bool Start (char ch)
		{
			TokenId = Token.UNKNOWN;

			if (ILTokenizer.IsIdStartChar (ch))
				TokenId = Token.ID;
			else if (ch == '\'')
				TokenId = Token.SQSTRING;
			else if (ch == '"')
				TokenId = Token.QSTRING;

			return TokenId != Token.UNKNOWN;
		}

		public override string Build ()
		{
			if (TokenId == Token.UNKNOWN)
				return string.Empty;
			
			var ch = 0;
			var reader = Tokenizer.Reader;
			var idsb = new StringBuilder ();
			
			if (TokenId == Token.SQSTRING || TokenId == Token.QSTRING) {
				var term = (TokenId == Token.SQSTRING) ? '\'' : '"';
				reader.Read (); // skip quote
				
				for (ch = reader.Read (); ch != -1; ch = reader.Read ()) {
					if (ch == term)
						break;

					if (ch == '\\') {
						ch = reader.Read ();

						/*
						 * Long string can be broken across multiple lines
						 * by using '\' as the last char in line.
						 * Any white space chars between '\' and the first
						 * char on the next line are ignored.
						 */
						if (ch == '\n') {
							reader.SkipWhitespace ();
							continue;
						}

						var escaped = Escape (reader, ch);
						if (escaped == -1) {
							reader.Unread (ch);
							ch = '\\';
						} else
							ch = escaped;
					}

					idsb.Append ((char) ch);
				}
			} else { // ID
				while ((ch = reader.Read ()) != -1)
					if (ILTokenizer.IsIdChar ((char) ch)) {
						idsb.Append ((char) ch);
					} else {
						reader.Unread (ch);
						break;
					}
			}
			
			return idsb.ToString ();
		}

		public static int Escape (ILReader reader, int ch)
		{
			int res = -1;

			if (ch >= '0' && ch <= '7') {
				var octal = new StringBuilder ();
				octal.Append ((char) ch);
				var possible_octal_char = reader.Peek ();
				
				if (possible_octal_char >= '0' && possible_octal_char <= '7') {
					octal.Append ((char) reader.Read ());
					possible_octal_char = reader.Peek ();
					
					if (possible_octal_char >= '0' && possible_octal_char <= '7')
						octal.Append ((char) reader.Read ());
				}
				
				res = Convert.ToInt32 (octal.ToString (), 8);
			} else {
				var id = "abfnrtv\"'\\".IndexOf ((char) ch);
				if (id != -1)
					res = "\a\b\f\n\r\t\v\"'\\" [id];
			}

			return res;
		}
	}
}
