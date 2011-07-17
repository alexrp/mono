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
	
	public sealed class Report {
		internal Report ()
		{
			MessageOutput = Console.Out;
			WarningOutput = Console.Out;
			ErrorOutput = Console.Error;
		}
		
		public TextWriter MessageOutput { get; set; }
		
		public TextWriter WarningOutput { get; set; }
		
		public TextWriter ErrorOutput { get; set; }
		
		public bool Quiet { get; set; }
		
		public bool Verbose { get; set; }

		public string FilePath { get; internal set; }
		
		internal ILTokenizer Tokenizer { get; set; }
		
		public event EventHandler<MessageEventArgs> Message;
		
		public event EventHandler<WarningEventArgs> Warning;
		
		public event EventHandler<ErrorEventArgs> Error;
		
		private Location GetLocation ()
		{
			if (Tokenizer != null)
				return Tokenizer.Reader.Location;
			
			return null;
		}

		internal void WriteError (Error error, string message, params object[] args)
		{
			WriteError (error, GetLocation (), message, args);
		}

		internal void WriteError (Error error, Location location, string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Error;
			if (evnt != null)
				evnt (null, new ErrorEventArgs (error, location, msg));
			
			throw new ILAsmException (error, msg, location, FilePath);
		}

		internal void WriteWarning (Warning warning, string message, params object[] args)
		{
			WriteWarning (warning, GetLocation (), message, args);
		}

		internal void WriteWarning (Warning warning, Location location, string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Warning;
			if (evnt != null)
				evnt (null, new WarningEventArgs (warning, location, msg));
			
			var locationStr = string.Empty;
			
			if (location != null)
				locationStr = FilePath + ":" + location.Line + ": ";
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			WarningOutput.WriteLine (string.Format ("{0}Warning ILW{1}: {2}", locationStr,
				((int) warning).ToString ("0000"), msg));
			Console.ResetColor ();
		}

		internal void WriteMessage (string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Message;
			if (evnt != null)
				evnt (null, new MessageEventArgs (msg));
			
			if (Quiet)
				return;
			
			MessageOutput.WriteLine (msg);
		}
		
		internal void WriteVerbose (string message, params object[] args)
		{
			var msg = string.Format (message, args);
			
			var evnt = Message;
			if (evnt != null)
				evnt (null, new MessageEventArgs (msg));
			
			if (!Verbose)
				return;
			
			MessageOutput.WriteLine (msg);
		}
	}
}
