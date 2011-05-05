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
		public Location Location { get; private set; }

		public string FilePath { get; private set; }

		public ILAsmException (string message, Location location, string filePath)
			: base (message)
		{
			this.FilePath = filePath;
			this.Location = location;
		}

		public ILAsmException (string message, Location location)
			: this (message, location, null)
		{
		}

		public ILAsmException (string message)
			: this (message, null)
		{
		}

		public ILAsmException (string message, Location location, string filePath, Exception inner)
			: base (message, inner)
		{
			this.FilePath = filePath;
			this.Location = location;
		}

		public ILAsmException (string message, Location location, Exception inner)
			: base (message, location, null, inner)
		{
		}

		public ILAsmException (string message, Exception inner)
			: base (message, null, null, inner)
		{
		}

		protected ILAsmException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}

		public override string ToString ()
		{
			var location_str = " : ";
			if (Location != null)
				location_str = " (" + Location.line + ", " + Location.column + ") : ";

			return String.Format ("{0}{1}Error : {2}",
				(FilePath != null ? FilePath : ""), location_str, Message);
		}
	}
}
