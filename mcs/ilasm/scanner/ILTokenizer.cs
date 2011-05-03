// ILTokenizer.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;

namespace Mono.ILAsm {
	public delegate void NewTokenEvent (object sender, NewTokenEventArgs args);

	public class NewTokenEventArgs : EventArgs {
		public ILToken Token { get; private set; }

		public NewTokenEventArgs (ILToken token)
		{
			Token = token;
		}
	}

	public class ILTokenizer : ITokenStream {
		private const string idchars = "_$@?.`";
		private static readonly Hashtable keywords;
		private static readonly Hashtable directives;
		private ILToken lastToken;
		private readonly ILReader reader;
		private readonly StringHelper strBuilder;
		private readonly NumberHelper numBuilder;
		internal bool in_byte_array;

		public event NewTokenEvent NewTokenEvent;

		static ILTokenizer ()
		{
			keywords = ILTables.Keywords;
			directives = ILTables.Directives;
		}

		public ILTokenizer (StreamReader reader)
		{
			this.reader = new ILReader (reader);
			strBuilder = new StringHelper (this);
			numBuilder = new NumberHelper (this);
			lastToken = ILToken.Invalid.Clone () as ILToken;
		}

		public ILReader Reader {
			get {
				return reader;
			}
		}

		public Location Location {
			get {
				return reader.Location;
			}
		}

		public ILToken GetNextToken ()
		{
			if (lastToken == ILToken.EOF)
				return ILToken.EOF;
			
			int ch;
			int next;
			ILToken res = ILToken.EOF.Clone () as ILToken;
			
			while ((ch = reader.Read ()) != -1) {
				// Comments
				if (ch == '/') {
					next = reader.Peek ();
					if (next == '/') {
						// double-slash comment, skip to the end of the line.
						for (reader.Read (); next != -1 && next != '\n'; next = reader.Read ())
							;
						continue;
					} else if (next == '*') {
						reader.Read ();
						for (next = reader.Read (); next != -1; next = reader.Read ()) {
							if (next == '*' && reader.Peek () == '/') {
								reader.Read ();
								goto end;
							}
						}
						
					end:
						continue;
					}
				}

				// HEXBYTES are flagged by the parser otherwise it is
				// impossible to figure them out
				if (in_byte_array) {
					var hx = string.Empty;

					if (char.IsWhiteSpace ((char) ch))
						continue;

					if (ch == ')') {
						res = ILToken.CloseParens;
						break;
					}

					if (!is_hex (ch))
						throw new ILTokenizingException (reader.Location, ((char) ch).ToString ());
					
					hx += (char) ch;
					if (is_hex (reader.Peek ()))
						hx += (char) reader.Read ();
					else if (!char.IsWhiteSpace ((char) reader.Peek ()) && reader.Peek () != ')')
						throw new ILTokenizingException (reader.Location, ((char) reader.Peek ()).ToString ());
					
					res.token = Token.HEXBYTE;
					res.val = byte.Parse (hx, NumberStyles.HexNumber);

					while (char.IsWhiteSpace ((char)reader.Peek ()))
						reader.Read ();
					
					break;
				}

				// Ellipsis
				if (ch == '.' && reader.Peek () == '.') {
					reader.MarkLocation ();
					int ch2 = reader.Read ();
					if (reader.Peek () == '.') {
						res = ILToken.Ellipsis;
						reader.Read ();
						break;
					}
					
					reader.Unread (ch2);
					reader.RestoreLocation ();
				}

				if (ch == '.' || ch == '#') {
					next = reader.Peek ();
					if (ch == '.' && char.IsDigit ((char) next)) {
						numBuilder.Start (ch);
						reader.Unread (ch);
						numBuilder.Build ();
						
						if (numBuilder.ResultToken != ILToken.Invalid) {
							res.CopyFrom (numBuilder.ResultToken);
							break;
						}
					} else {
						if (strBuilder.Start (next) && strBuilder.TokenId == Token.ID) {
							reader.MarkLocation ();
							string dirBody = strBuilder.Build ();
							var dir = new string ((char) ch, 1) + dirBody;
							if (IsDirective (dir)) {
								res = ILTables.Directives [dir] as ILToken;
							} else {
								reader.Unread (dirBody.ToCharArray ());
								reader.RestoreLocation ();
								res = ILToken.Dot;
							}
						} else {
							res = ILToken.Dot;
						}
						
						break;
					}
				}

				// Numbers && Hexbytes
				if (numBuilder.Start (ch)) {
					if ((ch == '-') && !(char.IsDigit ((char) reader.Peek ()))) {
						res = ILToken.Dash;
						break;
					} else {
						reader.Unread (ch);
						numBuilder.Build ();
						if (numBuilder.ResultToken != ILToken.Invalid) {
							res.CopyFrom (numBuilder.ResultToken);
							break;
						}
					}
				}

				// Punctuation
				var punct = ILToken.GetPunctuation (ch);
				if (punct != null) {
					if (punct == ILToken.Colon && reader.Peek () == ':') {
						reader.Read ();
						res = ILToken.DoubleColon;
					} else {
						res = punct;
					}
					break;
				}

				// ID | QSTRING | SQSTRING | INSTR_* | KEYWORD
				if (strBuilder.Start (ch)) {
					reader.Unread (ch);
					string val = strBuilder.Build ();
					
					if (strBuilder.TokenId == Token.ID) {
						ILToken opCode;
						next = reader.Peek ();
						if (next == '.') {
							reader.MarkLocation ();
							reader.Read ();
							next = reader.Peek ();
							
							if (IsIdChar ((char) next)) {
								string opTail = BuildId ();
								var full_str = string.Format ("{0}.{1}", val, opTail);
								opCode = InstrTable.GetToken (full_str);

								if (opCode == null) {
									if (strBuilder.TokenId != Token.ID) {
										reader.Unread (opTail.ToCharArray ());
										reader.Unread ('.');
										reader.RestoreLocation ();
										res.val = val;
									} else {
										res.token = Token.COMP_NAME;
										res.val = full_str;
									}
									
									break;
								} else {
									res = opCode;
									break;
								}
							} else if (char.IsWhiteSpace ((char) next)) {
								// Handle 'tail.' and 'unaligned.'
								opCode = InstrTable.GetToken (val + ".");
								if (opCode != null) {
									res = opCode;
									break;
								}
								
								// Let the parser handle the dot
								reader.Unread ('.');
							}
						}
						
						opCode = InstrTable.GetToken (val);
						if (opCode != null) {
							res = opCode;
							break;
						}
						
						if (IsKeyword (val)) {
							res = ILTables.Keywords [val] as ILToken;
							break;
						}
					}

					res.token = strBuilder.TokenId;
					res.val = val;
					break;
				}
			}

			OnNewToken (res);
			lastToken.CopyFrom (res);
			return res;
		}

		public ILToken NextToken {
			get {
				return GetNextToken ();
			}
		}

		public ILToken LastToken {
			get {
				return lastToken;
			}
		}

		bool is_hex (int e)
		{
			return (e >= '0' && e <= '9') || (e >= 'A' && e <= 'F') || (e >= 'a' && e <= 'f');
		}

		private static bool IsIdStartChar (char ch)
		{
			return (char.IsLetter (ch) || (idchars.IndexOf (ch) != -1));
		}

		private static bool IsIdChar (char ch)
		{
			return (char.IsLetterOrDigit (ch) || (idchars.IndexOf (ch) != -1));
		}

		public static bool IsOpCode (string name)
		{
			return InstrTable.IsInstr (name);
		}

		public static bool IsDirective (string name)
		{
			char ch = name [0];
			bool res = (ch == '.' || ch == '#');

			if (res)
				res = directives.Contains (name);

			return res;
		}

		private string BuildId ()
		{
			var idsb = new StringBuilder ();
			int ch, last;

			last = -1;
			while ((ch = reader.Read ()) != -1) {
				if (IsIdChar ((char) ch) || ch == '.') {
					idsb.Append ((char) ch);
				} else {
					reader.Unread (ch);
					// Never end an id on a DOT
					if (last == '.') {
						reader.Unread (last);
						idsb.Length -= 1;
					}
					
					break;
				}
				
				last = ch;
			}

			return idsb.ToString ();
		}

		public static bool IsKeyword (string name)
		{
			return keywords.Contains (name);
		}

		private void OnNewToken (ILToken token)
		{
			var evnt = NewTokenEvent;
			if (evnt != null)
				evnt (this, new NewTokenEventArgs (token));
		}
	}
}
