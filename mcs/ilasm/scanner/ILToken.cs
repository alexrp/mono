// ILToken.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;

namespace Mono.ILAsm {
	internal class ILToken : ICloneable {
		public static readonly ILToken Invalid = new ILToken (-1, "invalid");
		public static readonly ILToken EOF = new ILToken (Token.EOF, "eof");
		public static readonly ILToken Dot = new ILToken (Token.DOT, ".");
		public static readonly ILToken OpenBrace = new ILToken (Token.OPEN_BRACE, "{");
		public static readonly ILToken CloseBrace = new ILToken (Token.CLOSE_BRACE, "}");
		public static readonly ILToken OpenBracket = new ILToken (Token.OPEN_BRACKET, "[");
		public static readonly ILToken CloseBracket = new ILToken (Token.CLOSE_BRACKET, "]");
		public static readonly ILToken OpenParens = new ILToken (Token.OPEN_PARENS, "(");
		public static readonly ILToken CloseParens = new ILToken (Token.CLOSE_PARENS, ")");
		public static readonly ILToken Comma = new ILToken (Token.COMMA, ",");
		public static readonly ILToken Colon = new ILToken (Token.COLON, ":");
		public static readonly ILToken DoubleColon = new ILToken (Token.DOUBLE_COLON, "::");
		public static readonly ILToken Semicolon = new ILToken (Token.SEMICOLON, ";");
		public static readonly ILToken Assign = new ILToken (Token.ASSIGN, "=");
		public static readonly ILToken Star = new ILToken (Token.STAR, "*");
		public static readonly ILToken Ampersand = new ILToken (Token.AMPERSAND, "&");
		public static readonly ILToken Plus = new ILToken (Token.PLUS, "+");
		public static readonly ILToken Slash = new ILToken (Token.SLASH, "/");
		public static readonly ILToken Bang = new ILToken (Token.BANG, "!");
		public static readonly ILToken Ellipsis = new ILToken (Token.ELLIPSIS, "...");
		public static readonly ILToken Dash = new ILToken (Token.DASH, "-");
		public static readonly ILToken OpenAngleBracket = new ILToken (Token.OPEN_ANGLE_BRACKET, "<");
		public static readonly ILToken CloseAngleBracket = new ILToken (Token.CLOSE_ANGLE_BRACKET, ">");
		static readonly ILToken[] punctuations = new[] {
			OpenBrace,
			CloseBrace,
			OpenBracket,
			CloseBracket,
			OpenParens,
			CloseParens,
			Comma,
			Colon,
			Semicolon,
			Assign,
			Star,
			Ampersand,
			Plus,
			Slash,
			Bang,
			OpenAngleBracket,
			CloseAngleBracket,
		};

		public ILToken ()
		{
		}

		public ILToken (int token, object val)
		{
			TokenId = token;
			Value = val;
		}

		public ILToken (ILToken that)
		{
			TokenId = that.TokenId;
			Value = that.Value;
		}

		public int TokenId { get; private set; }

		public object Value { get; private set; }

		public virtual object Clone ()
		{
			return new ILToken (this);
		}

		public override int GetHashCode ()
		{
			var h = TokenId;
			if (Value != null)
				h ^= Value.GetHashCode ();
			
			return h;
		}

		public override string ToString ()
		{
			return TokenId.ToString () + " : " + (Value != null ? Value.ToString () : "<null>");
		}

		public override bool Equals (object o)
		{
			var res = o != null;

			if (res) {
				res = ReferenceEquals (this, o);
				if (!res) {
					res = o is ILToken;
					if (res) {
						var that = o as ILToken;
						res = TokenId == that.TokenId && Value.Equals (that.Value);
					}
				}
			}

			return res;
		}

		static bool EqImpl (ILToken t1, ILToken t2)
		{
			var res = false;
			
			if (t1 as object != null)
				res = t1.Equals (t2);
			else
				res = t2 as object == null;

			return res;
		}

		public static bool operator == (ILToken t1, ILToken t2)
		{
			return EqImpl (t1, t2);
		}

		public static bool operator != (ILToken t1, ILToken t2)
		{
			return !EqImpl (t1, t2);
		}

		public static ILToken GetPunctuation (int ch)
		{
			var id = "{}[](),:;=*&+/!<>".IndexOf ((char) ch);
			ILToken res = null;

			if (id != -1)
				res = punctuations [id];

			return res;
		}
	}
}
