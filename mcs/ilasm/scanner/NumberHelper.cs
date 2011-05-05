// NumberHelper.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.Text;
using System.Globalization;

namespace Mono.ILAsm {
	internal sealed class NumberHelper : StringHelperBase {
		public NumberHelper (ILTokenizer host)
			: base (host)
		{
			Reset ();
		}

		private void Reset ()
		{
			ResultToken = ILToken.Invalid.Clone () as ILToken;
		}

		public override bool Start (char ch)
		{
			bool res = (char.IsDigit (ch) || ch == '-' || (ch == '.' && char.IsDigit ((char) host.Reader.Peek ())));
			Reset ();
			return res;
		}

		private bool is_hex (int e)
		{
			return (e >= '0' && e <= '9') || (e >= 'A' && e <= 'F') || (e >= 'a' && e <= 'f');
		}

		private bool is_sign (int ch)
		{
			return ((ch == '+') || (ch == '-'));
		}

		private bool is_e (int ch)
		{
			return ((ch == 'e') || (ch == 'E'));
		}

		public override string Build ()
		{
			var reader = host.Reader;
			reader.MarkLocation ();
			var num_builder = new StringBuilder ();
			var is_real = false;
			var dec_found = false;
			var nstyles = NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint |
				NumberStyles.AllowLeadingSign;

			int ch = reader.Read ();
			int peek = reader.Peek ();
			reader.Unread (ch);

			if (ch == '0' && (peek == 'x' || peek == 'X'))
				return BuildHex ();

			if (is_sign (reader.Peek ()))
				num_builder.Append ((char) reader.Read ());

			do {
				ch = reader.Read ();
				peek = reader.Peek ();
				num_builder.Append ((char) ch);

				if (is_e (ch)) {
					if (is_real)
						throw new ILTokenizingException (reader.Location, num_builder.ToString ());

					is_real = true;
				}
				if (ch == '.')
					dec_found = true;
				if (!is_hex (peek) && !(peek == '.' && !dec_found) && !is_e (peek) &&
					!(is_sign (peek) && is_real))
					break;
			} while (ch != -1);

			var num = num_builder.ToString ();

			// Check for hexbytes
			if (num.Length == 2)
			if (char.IsLetter (num [0]) || char.IsLetter (num [1])) {
				ResultToken.token = Token.HEXBYTE;
				ResultToken.val = byte.Parse (num, NumberStyles.HexNumber);
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
					var i = long.Parse (num, nstyles);
					ResultToken.token = Token.INT64;
					ResultToken.val = i;

					return num;
				} catch {
				}

				try {
					var i = (long) ulong.Parse (num, nstyles);
					ResultToken.token = Token.INT64;
					ResultToken.val = i;

					return num;
				} catch {
				}
			}

			try {
				var d = double.Parse (num, nstyles, NumberFormatInfo.InvariantInfo);
				ResultToken.token = Token.FLOAT64;
				ResultToken.val = d;
			} catch {
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
			var reader = host.Reader;
			reader.MarkLocation ();
			var num_builder = new StringBuilder ();
			var nstyles = NumberStyles.HexNumber;
			int peek;

			int ch = reader.Read ();
			if (ch != '0')
				throw new ILTokenizingException (reader.Location, ((char) ch).ToString ());

			ch = reader.Read ();

			if (ch != 'x' && ch != 'X')
				throw new ILTokenizingException (reader.Location, "0" + (char) ch);

			do {
				ch = reader.Read ();
				peek = reader.Peek ();
				num_builder.Append ((char) ch);

				if (!is_hex ((char) peek))
					break;

				if (num_builder.Length == 32)
					throw new ILTokenizingException (reader.Location, num_builder.ToString ());

			} while (ch != -1);

			string num = num_builder.ToString ();

			try {
				var i = (long) ulong.Parse (num, nstyles);
				//if (i < int.MinValue || i > int.MaxValue) {
				ResultToken.token = Token.INT64;
				ResultToken.val = i;
				//} else {
				//	ResultToken.token = Token.INT32;
				//	ResultToken.val = (int) i;
				//}
			} catch {
				string tnum = num;
				reader.Unread (num.ToCharArray ());
				reader.RestoreLocation ();
				num = string.Empty;
				Reset ();
				throw new ILTokenizingException (reader.Location, tnum);
			}
			
			return num;
		}

		public ILToken ResultToken { get; private set; }
	}
}
