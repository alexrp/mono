// StringHelperBase.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.Text;

namespace Mono.ILAsm {
	internal abstract class StringHelperBase {
		protected ILTokenizer Tokenizer { get; private set; }

		public int TokenId { get; protected set; }

		public StringHelperBase (ILTokenizer host)
		{
			Tokenizer = host;
			TokenId = Token.UNKNOWN;
		}

		public abstract bool Start (char ch);

		public bool Start (int ch)
		{
			return Start ((char) ch);
		}

		public bool Start ()
		{
			return Start (Tokenizer.Reader.Peek ());
		}

		public abstract string Build ();
	}
}
