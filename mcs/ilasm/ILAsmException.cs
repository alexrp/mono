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
			: this (message, location, (string)null)
		{
		}

		public ILAsmException (string message)
			: this (message, (Location)null)
		{
		}

		public ILAsmException (string message, Location location, string filePath, Exception inner)
			: base (message, inner)
		{
			this.FilePath = filePath;
			this.Location = location;
		}

		public ILAsmException (string message, Location location, Exception inner)
			: this (message, location, (string)null, inner)
		{
		}

		public ILAsmException (string message, Exception inner)
			: this (message, (Location)null, (string)null, inner)
		{
		}

		protected ILAsmException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}

		public override string ToString ()
		{
			var location_str = string.Empty;
			if (Location != null)
				location_str = FilePath + ":" + Location.line + "," +
					Location.column + ": ";

			return string.Format ("{0}Error: {1}",
				location_str, Message);
		}
	}
}
