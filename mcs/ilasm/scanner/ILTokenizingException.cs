//
// Mono.ILASM.ILTokenizingException
//
// Author(s):
//  Jackson Harper (jackson@ximian.com)
//
// Copyright 2004 Novell, Inc (http://www.novell.com)
//
using System;

namespace Mono.ILAsm {
	[Serializable]
	public class ILTokenizingException : ILAsmException {
		public string Token { get; private set; }


		public ILTokenizingException (Location location, string token)
			: base (token, location)
		{
			Token = token;
		}


		public ILTokenizingException (Location location, string token, Exception inner)
			: base (token, location, inner)
		{
		}


		protected ILTokenizingException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
	}
}
