// ScannerAdapter.cs
// (C) Sergey Chaban (serge@wildwestsoftware.com)
using System;

namespace Mono.ILAsm {
	public sealed class ScannerAdapter : yyParser.yyInput {
		public ScannerAdapter (ILTokenizer tokens)
		{
			this.BaseStream = tokens;
		}

		public ILTokenizer BaseStream { get; private set; }

		//
		// yyParser.yyInput interface
		//
		
		public bool advance ()
		{
			return BaseStream.GetNextToken () != ILToken.EOF;
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
