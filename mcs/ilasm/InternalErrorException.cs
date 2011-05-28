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
	internal class InternalErrorException : Exception {
		public InternalErrorException (string message)
			: base (message)
		{
		}

		public InternalErrorException (string message, Exception inner)
			: base (message, inner)
		{
		}

		protected InternalErrorException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
	}
}
