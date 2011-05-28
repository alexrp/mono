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
	public class MessageEventArgs : EventArgs {
		public string Message { get; private set; }
		
		public MessageEventArgs (string message)
		{
			Message = message;
		}
	}
	
	public class WarningEventArgs : MessageEventArgs {
		public Location Location { get; private set; }
		
		public WarningEventArgs (Location location, string message)
			: base (message)
		{
			Location = location;
		}
	}
	
	public class ErrorEventArgs : WarningEventArgs {
		public Error Error { get; private set; }
		
		public ErrorEventArgs (Error error, Location location, string message)
			: base (location, message)
		{
			Error = error;
		}
	}
	
	public static class Report {
		static Report ()
		{
			MessageOutput = Console.Out;
			WarningOutput = Console.Out;
			ErrorOutput = Console.Error;
		}
		
		public static TextWriter MessageOutput { get; set; }
		
		public static TextWriter WarningOutput { get; set; }
		
		public static TextWriter ErrorOutput { get; set; }
		
		public static bool Quiet { get; set; }

		public static string FilePath { get; internal set; }
		
		public static ILTokenizer Tokenizer { get; internal set; }
		
		public static event EventHandler<MessageEventArgs> Message;
		
		public static event EventHandler<WarningEventArgs> Warning;
		
		public static event EventHandler<ErrorEventArgs> Error;

		internal static void WriteError (Error error, string message, params object[] args)
		{
			WriteError (error, null, message, args);
		}

		internal static void WriteError (Error error, Location location, string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Error;
			if (evnt != null)
				evnt (null, new ErrorEventArgs (error, location, msg));
			
			throw new ILAsmException (error, FilePath, location, msg);
		}

		internal static void WriteWarning (string message, params object[] args)
		{
			WriteWarning (null, message, args);
		}

		internal static void WriteWarning (Location location, string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Warning;
			if (evnt != null)
				evnt (null, new WarningEventArgs (location, msg));
			
			var location_str = string.Empty;
			if (location != null)
				location_str = FilePath + ":" + location.line + "," +
					location.column + ": ";
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			WarningOutput.WriteLine (string.Format ("{0}Warning: {1}", location_str, msg));
			Console.ResetColor ();
		}

		internal static void WriteMessage (string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Message;
			if (evnt != null)
				evnt (null, new MessageEventArgs (msg));
			
			if (Quiet)
				return;
			
			MessageOutput.WriteLine (msg);
		}
	}
}
