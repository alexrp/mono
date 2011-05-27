// ScannerAdapter.cs
// (C) Sergey Chaban (serge@wildwestsoftware.com)
using System;

namespace Mono.ILAsm {
	public sealed class ScannerAdapter : yyParser.yyInput {
		public ScannerAdapter (ITokenStream tokens)
		{
			this.BaseStream = tokens;
		}

		public ITokenStream BaseStream { get; private set; }

		//
		// yyParser.yyInput interface
		//
		
		public bool advance ()
		{
			return BaseStream.NextToken != ILToken.EOF;
		}

		public int token ()
		{
			return BaseStream.LastToken.TokenId;
		}

		public object value ()
		{
			return BaseStream.LastToken.Value;
		}
	}
}
