// StringHelperBase.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.Text;

namespace Mono.ILAsm {
	internal abstract class StringHelperBase {
		protected ILTokenizer host;

		public StringHelperBase (ILTokenizer host)
		{
			this.host = host;
			this.TokenId = Token.UNKNOWN;
		}

		public abstract bool Start (char ch);

		public bool Start (int ch)
		{
			return Start ((char) ch);
		}

		public bool Start ()
		{
			return Start (host.Reader.Peek ());
		}

		public abstract string Build ();

		public int TokenId { get; protected set; }
	}
}
