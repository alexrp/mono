//
// Mono.ILAsm.InternalErrorException
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//  Alex Rønne Petersen (alex@alexrp.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//
using System;
using System.Runtime.Serialization;

namespace Mono.ILAsm {
	[Serializable]
	internal class InternalErrorException : ILAsmException {
		public InternalErrorException (string message)
			: base (Error.InternalError, message)
		{
		}

		public InternalErrorException (string message, Exception inner)
			: base (Error.InternalError, message, inner)
		{
		}

		protected InternalErrorException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
	}
}
