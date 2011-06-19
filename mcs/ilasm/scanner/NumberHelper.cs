// NumberHelper.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.Text;
using System.Globalization;

namespace Mono.ILAsm {
	internal sealed class NumberHelper : StringHelperBase {
		public ILToken ResultToken { get; private set; }

		public NumberHelper (ILTokenizer host)
			: base (host)
		{
			Reset ();
		}

		void Reset ()
		{
			ResultToken = ILToken.Invalid.Clone () as ILToken;
		}

		public override bool Start (char ch)
		{
			var res = (char.IsDigit (ch) || ch == '-' || (ch == '.' && char.IsDigit ((char) Tokenizer.Reader.Peek ())));
			Reset ();
			return res;
		}

		public override string Build ()
		{
			var reader = Tokenizer.Reader;
			reader.MarkLocation ();
			var num_builder = new StringBuilder ();
			var is_real = false;
			var dec_found = false;
			var nstyles = NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint |
				NumberStyles.AllowLeadingSign;

			var ch = reader.Read ();
			var peek = reader.Peek ();
			reader.Unread (ch);

			if (ch == '0' && (peek == 'x' || peek == 'X'))
				return BuildHex ();

			if (ILTokenizer.IsSign ((char) reader.Peek ()))
				num_builder.Append ((char) reader.Read ());

			do {
				ch = reader.Read ();
				peek = reader.Peek ();
				num_builder.Append ((char) ch);

				if (ILTokenizer.IsE ((char) ch)) {
					if (is_real)
						throw new ILTokenizingException (reader.Location, num_builder.ToString ());

					is_real = true;
				}
				if (ch == '.')
					dec_found = true;
				if (!ILTokenizer.IsHex ((char) peek) && !(peek == '.' && !dec_found) &&
					!ILTokenizer.IsE ((char) peek) && !(ILTokenizer.IsSign ((char) peek) && is_real))
					break;
			} while (ch != -1);

			var num = num_builder.ToString ();

			// Check for hexbytes
			if (num.Length == 2)
			if (char.IsLetter (num [0]) || char.IsLetter (num [1])) {
				ResultToken = new ILToken (Token.HEXBYTE, byte.Parse (num, NumberStyles.HexNumber));
				return num;
			}

			if (ch == '.' && peek == '.') {
				num = num.Substring (0, num.Length - 1);
				reader.Unread ('.');
				dec_found = false;
			} else if (ch == '.')
				num += '0';

			if (!dec_found && !is_real) {
				try {
					ResultToken = new ILToken (Token.INT64, long.Parse (num, nstyles));
					return num;
				} catch (Exception) {
				}

				try {
					ResultToken = new ILToken (Token.INT64, (long) ulong.Parse (num, nstyles));
					return num;
				} catch (Exception) {
				}
			}

			try {
				ResultToken = new ILToken (Token.FLOAT64, double.Parse (num, nstyles, NumberFormatInfo.InvariantInfo));
			} catch (Exception) {
				reader.Unread (num.ToCharArray ());
				reader.RestoreLocation ();
				num = string.Empty;
				Reset ();
				throw new ILTokenizingException (reader.Location, num_builder.ToString ());
			}
			
			return num;
		}

		public string BuildHex ()
		{
			var reader = Tokenizer.Reader;
			reader.MarkLocation ();
			var num_builder = new StringBuilder ();
			var nstyles = NumberStyles.HexNumber;
			int peek;

			var ch = reader.Read ();
			if (ch != '0')
				throw new ILTokenizingException (reader.Location, ((char) ch).ToString ());

			ch = reader.Read ();

			if (ch != 'x' && ch != 'X')
				throw new ILTokenizingException (reader.Location, "0" + (char) ch);

			do {
				ch = reader.Read ();
				peek = reader.Peek ();
				num_builder.Append ((char) ch);

				if (!ILTokenizer.IsHex ((char) peek))
					break;

				if (num_builder.Length == 32)
					throw new ILTokenizingException (reader.Location, num_builder.ToString ());
			} while (ch != -1);

			var num = num_builder.ToString ();

			try {
				ResultToken = new ILToken (Token.INT64, (long) ulong.Parse (num, nstyles));
			} catch (Exception) {
				var tnum = num;
				reader.Unread (num.ToCharArray ());
				reader.RestoreLocation ();
				num = string.Empty;
				Reset ();
				throw new ILTokenizingException (reader.Location, tnum);
			}
			
			return num;
		}
	}
}
