//
// Mono.ILAsm.ILAsmException
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
	public class ILAsmException : Exception {
		public Error Error { get; private set; }
		
		public Location Location { get; private set; }

		public string FilePath { get; private set; }

		public ILAsmException (Error error, string message, Location location, string filePath)
			: base (message)
		{
			Error = error;
			FilePath = filePath;
			Location = location;
		}

		public ILAsmException (Error error, string message, Location location)
			: this (error, message, location, (string) null)
		{
		}

		public ILAsmException (Error error, string message)
			: this (error, message, (Location) null)
		{
		}

		public ILAsmException (Error error, string message, Location location, string filePath, Exception inner)
			: base (message, inner)
		{
			Error = error;
			FilePath = filePath;
			Location = location;
		}

		public ILAsmException (Error error, string message, Location location, Exception inner)
			: this (error, message, location, (string) null, inner)
		{
		}

		public ILAsmException (Error error, string message, Exception inner)
			: this (error, message, (Location) null, (string) null, inner)
		{
		}

		protected ILAsmException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}

		public override string ToString ()
		{
			var locationStr = string.Empty;
			
			if (Location != null)
				locationStr = FilePath + ":" + Location.Line + ": ";

			return string.Format ("{0}Error ILE{1}: {2}",
				locationStr, ((int) Error).ToString ("0000"), Message);
		}
	}
}
