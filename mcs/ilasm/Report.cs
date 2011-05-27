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
		public static bool Quiet { get; set; }

		public static string FilePath { get; internal set; }

		public static void AssembleFile (string file, string target, string output)
		{
			if (Quiet)
				return;
			
			Console.WriteLine ("Assembling {0} to {1} -> {2}...", file, target, output);
			Console.WriteLine ();
		}

		public static void Error (Error error, string message, params object[] args)
		{
			Error (error, null, message, args);
		}

		public static void Error (Error error, Location location, string message, params object[] args)
		{
			throw new ILAsmException (error, FilePath, location, string.Format (message, args));
		}

		public static void Warning (string message, params object[] args)
		{
			Warning (null, message, args);
		}

		public static void Warning (Location location, string message, params object[] args)
		{
			var location_str = string.Empty;
			if (location != null)
				location_str = FilePath + ":" + location.line + "," +
					location.column + ": ";
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Error.WriteLine (string.Format ("{0}Warning: {1}",
				location_str, string.Format (message, args)));
			Console.ResetColor ();
		}

		public static void Message (string message, params object[] args)
		{
			if (Quiet)
				return;
			
			Console.WriteLine (message);
		}
	}
}
