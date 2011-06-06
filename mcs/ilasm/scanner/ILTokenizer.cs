// ILTokenizer.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace Mono.ILAsm {
	public class NewTokenEventArgs : EventArgs {
		public ILToken Token { get; private set; }

		public NewTokenEventArgs (ILToken token)
		{
			Token = token;
		}
	}

	public class ILTokenizer {
		const string id_chars = "_$@?.`";
		ILToken last_token;
		readonly StringHelper str_builder;
		readonly NumberHelper num_builder;
		internal bool in_byte_array;

		public event EventHandler<NewTokenEventArgs> NewToken;

		public ILTokenizer (StreamReader reader)
		{
			Reader = new ILReader (reader);
			str_builder = new StringHelper (this);
			num_builder = new NumberHelper (this);
			last_token = ILToken.Invalid.Clone () as ILToken;
		}

		public ILReader Reader { get; private set; }

		public ILToken GetNextToken ()
		{
			if (last_token == ILToken.EOF)
				return ILToken.EOF;
			
			int ch;
			int next;
			var res = ILToken.EOF.Clone () as ILToken;
			
			while ((ch = Reader.Read ()) != -1) {
				// Comments
				if (ch == '/') {
					next = Reader.Peek ();
					if (next == '/') {
						// double-slash comment, skip to the end of the line.
						for (Reader.Read (); next != -1 && next != '\n'; next = Reader.Read ())
							;
						continue;
					} else if (next == '*') {
						Reader.Read ();
						for (next = Reader.Read (); next != -1; next = Reader.Read ())
							if (next == '*' && Reader.Peek () == '/') {
								Reader.Read ();
								goto end;
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

					if (!IsHex (ch))
						throw new ILTokenizingException (Reader.Location, ((char) ch).ToString ());
					
					hx += (char) ch;
					if (IsHex (Reader.Peek ()))
						hx += (char) Reader.Read ();
					else if (!char.IsWhiteSpace ((char) Reader.Peek ()) && Reader.Peek () != ')')
						throw new ILTokenizingException (Reader.Location, ((char) Reader.Peek ()).ToString ());
					
					res.token = Token.HEXBYTE;
					res.val = byte.Parse (hx, NumberStyles.HexNumber);

					while (char.IsWhiteSpace ((char) Reader.Peek ()))
						Reader.Read ();
					
					break;
				}

				// Ellipsis
				if (ch == '.' && Reader.Peek () == '.') {
					Reader.MarkLocation ();
					var ch2 = Reader.Read ();
					if (Reader.Peek () == '.') {
						res = ILToken.Ellipsis;
						Reader.Read ();
						break;
					}
					
					Reader.Unread (ch2);
					Reader.RestoreLocation ();
				}

				if (ch == '.' || ch == '#') {
					next = Reader.Peek ();
					if (ch == '.' && char.IsDigit ((char) next)) {
						num_builder.Start (ch);
						Reader.Unread (ch);
						num_builder.Build ();
						
						if (num_builder.ResultToken != ILToken.Invalid) {
							res.CopyFrom (num_builder.ResultToken);
							break;
						}
					} else {
						if (str_builder.Start (next) && str_builder.TokenId == Token.ID) {
							Reader.MarkLocation ();
							var dir_body = str_builder.Build ();
							var dir = new string ((char) ch, 1) + dir_body;
							if (IsDirective (dir))
								res = ILTables.Directives.TryGet (dir);
							else {
								Reader.Unread (dir_body.ToCharArray ());
								Reader.RestoreLocation ();
								res = ILToken.Dot;
							}
						} else
							res = ILToken.Dot;
						
						break;
					}
				}

				// Numbers && Hexbytes
				if (num_builder.Start (ch))
				if (ch == '-' && !char.IsDigit ((char) Reader.Peek ())) {
					res = ILToken.Dash;
					break;
				} else {
					Reader.Unread (ch);
					num_builder.Build ();
					if (num_builder.ResultToken != ILToken.Invalid) {
						res.CopyFrom (num_builder.ResultToken);
						break;
					}
				}

				// Punctuation
				var punct = ILToken.GetPunctuation (ch);
				if (punct != null) {
					if (punct == ILToken.Colon && Reader.Peek () == ':') {
						Reader.Read ();
						res = ILToken.DoubleColon;
					} else
						res = punct;
					break;
				}

				// ID | QSTRING | SQSTRING | INSTR_* | KEYWORD
				if (str_builder.Start (ch)) {
					Reader.Unread (ch);
					var val = str_builder.Build ();
					
					if (str_builder.TokenId == Token.ID) {
						ILToken opCode;
						next = Reader.Peek ();
						if (next == '.') {
							Reader.MarkLocation ();
							Reader.Read ();
							next = Reader.Peek ();
							
							if (IsIdChar ((char) next)) {
								var opTail = BuildId ();
								var fullStr = string.Format ("{0}.{1}", val, opTail);
								opCode = ILTables.OpCodes.TryGet (fullStr);

								if (opCode == null) {
									if (str_builder.TokenId != Token.ID) {
										Reader.Unread (opTail.ToCharArray ());
										Reader.Unread ('.');
										Reader.RestoreLocation ();
										res.val = val;
									} else {
										res.token = Token.COMP_NAME;
										res.val = fullStr;
									}
									
									break;
								} else {
									res = opCode;
									break;
								}
							} else if (char.IsWhiteSpace ((char) next)) {
								// Handle 'tail.' and 'unaligned.'
								opCode = ILTables.OpCodes.TryGet (val + ".");
								if (opCode != null) {
									res = opCode;
									break;
								}
								
								// Let the parser handle the dot
								Reader.Unread ('.');
							}
						}
						
						opCode = ILTables.OpCodes.TryGet (val);
						if (opCode != null) {
							res = opCode;
							break;
						}
						
						if (IsKeyword (val)) {
							res = ILTables.Keywords.TryGet (val);
							break;
						}
					}

					res.token = str_builder.TokenId;
					res.val = val;
					break;
				}
			}

			OnNewToken (res);
			last_token.CopyFrom (res);
			return res;
		}

		public ILToken LastToken {
			get {
				return last_token;
			}
		}

		bool IsHex (int e)
		{
			return (e >= '0' && e <= '9') || (e >= 'A' && e <= 'F') || (e >= 'a' && e <= 'f');
		}

		static bool IsIdStartChar (char ch)
		{
			return (char.IsLetter (ch) || (id_chars.IndexOf (ch) != -1));
		}

		static bool IsIdChar (char ch)
		{
			return (char.IsLetterOrDigit (ch) || (id_chars.IndexOf (ch) != -1));
		}

		public static bool IsOpCode (string name)
		{
			return ILTables.OpCodes.ContainsKey (name);
		}

		public static bool IsDirective (string name)
		{
			var ch = name [0];

			if (ch == '.' || ch == '#')
				return ILTables.Directives.ContainsKey (name);

			return false;
		}

		public static bool IsKeyword (string name)
		{
			return ILTables.Keywords.ContainsKey (name);
		}

		string BuildId ()
		{
			var idsb = new StringBuilder ();
			int ch;
			int last;

			last = -1;
			while ((ch = Reader.Read ()) != -1) {
				if (IsIdChar ((char) ch) || ch == '.')
					idsb.Append ((char) ch);
				else {
					Reader.Unread (ch);
					// Never end an id on a DOT
					if (last == '.') {
						Reader.Unread (last);
						idsb.Length -= 1;
					}
					
					break;
				}
				
				last = ch;
			}

			return idsb.ToString ();
		}

		void OnNewToken (ILToken token)
		{
			var evnt = NewToken;
			if (evnt != null)
				evnt (this, new NewTokenEventArgs (token));
		}
	}
}
