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
		public Warning Warning { get; private set; }
		
		public Location Location { get; private set; }
		
		public WarningEventArgs (Warning warning, Location location, string message)
			: base (message)
		{
			Warning = warning;
			Location = location;
		}
	}
	
	public class ErrorEventArgs : MessageEventArgs {
		public Error Error { get; private set; }
		
		public Location Location { get; private set; }
		
		public ErrorEventArgs (Error error, Location location, string message)
			: base (message)
		{
			Error = error;
			Location = location;
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
		
		private static Location GetLocation ()
		{
			if (Tokenizer != null)
				return Tokenizer.Reader.Location;
			
			return null;
		}

		internal static void WriteError (Error error, string message, params object[] args)
		{
			WriteError (error, GetLocation (), message, args);
		}

		internal static void WriteError (Error error, Location location, string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Error;
			if (evnt != null)
				evnt (null, new ErrorEventArgs (error, location, msg));
			
			throw new ILAsmException (error, msg, location, FilePath);
		}

		internal static void WriteWarning (Warning warning, string message, params object[] args)
		{
			WriteWarning (warning, GetLocation (), message, args);
		}

		internal static void WriteWarning (Warning warning, Location location, string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Warning;
			if (evnt != null)
				evnt (null, new WarningEventArgs (warning, location, msg));
			
			var location_str = string.Empty;
			if (location != null)
				location_str = FilePath + ":" + location.Line + "," +
					location.Column + ": ";
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			WarningOutput.WriteLine (string.Format ("{0}Warning ILW{1}: {2}", location_str,
				((int) warning).ToString ("0000"), msg));
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
