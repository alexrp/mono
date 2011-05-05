//
// Mono.ILAsm.Report
//
// Author(s):
//  Jackson Harper (Jackson@LatitudeGeo.com)
//  Alex Rønne Petersen (alex@alexrp.com)
//
// (C) 2003 Jackson Harper, All rights reserved
//
using System;
using System.IO;

namespace Mono.ILAsm {
	public abstract class Report {
		static Report ()
		{
			ErrorCount = 0;
			Quiet = false;
		}

		public static int ErrorCount { get; private set; }

		public static bool Quiet { get; set; }

		public static string FilePath { get; internal set; }

		public static void AssembleFile (string file, string listing,
						string target, string output)
		{
			if (quiet)
				return;
			
			Console.WriteLine ("Assembling '{0}' , {1}, to {2} --> '{3}'", file,
				GetListing (listing), target, output);
			Console.WriteLine ();
		}

		public static void Error (string message)
		{
			Error (null, message);
		}

		public static void Error (Location location, string message)
		{
			ErrorCount++;
			throw new ILAsmException (FilePath, location, message);
		}

		public static void Warning (string message)
		{
			Warning (null, message);
		}

		public static void Warning (Location location, string message)
		{
			var location_str = " : ";
			if (location != null)
				location_str = " (" + location.line + ", " + location.column + ") : ";

			Console.Error.WriteLine (String.Format ("{0}{1}Warning -- {2}",
				(FilePath != null ? FilePath : ""), location_str, message));
		}

		public static void Message (string message)
		{
			if (quiet)
				return;
			
			Console.WriteLine (message);
		}

		private static string GetListing (string listing)
		{
			if (listing == null)
				return "no listing file";
			
			return listing;
		}
	}
}
