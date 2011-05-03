// ILReader.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.IO;
using System.Text;
using System.Collections;

namespace Mono.ILAsm {
	public sealed class ILReader {
		private readonly StreamReader reader;
		private readonly Stack putback_stack;
		private Location location;
		private Location markedLocation;

		public ILReader (StreamReader reader)
		{
			this.reader = reader;
			putback_stack = new Stack ();

			location = new Location ();
			markedLocation = Location.Unknown;
		}

		public Location Location {
			get {
				return location;
			}
		}

		public StreamReader BaseReader {
			get {
				return reader;
			}
		}

		private int DoRead ()
		{
			if (putback_stack.Count > 0) 
				return (char) putback_stack.Pop ();

			return reader.Read ();
		}

		private int DoPeek ()
		{
			if (putback_stack.Count > 0)
				return (char) putback_stack.Peek ();

			return reader.Peek ();
		}

		public int Read ()
		{
			int read = DoRead ();
			
			if (read == '\n')
				location.NewLine ();
			else
				location.NextColumn ();
			
			return read;
		}

		public int Peek ()
		{
			return DoPeek ();
		}

		public void Unread (char c)
		{
			putback_stack.Push (c);

			if ('\n' == c)
				location.PreviousLine ();

			location.PreviousColumn ();
		}

		public void Unread (char[] chars)
		{
			for (int i=chars.Length-1; i>=0; i--)
				Unread (chars [i]);
		}

		public void Unread (int c)
		{
			Unread ((char) c);
		}

		public void SkipWhitespace ()
		{
			int ch = Read ();
			
			for (; ch != -1 && char.IsWhiteSpace((char) ch); ch = Read ())
				;
			
			if (ch != -1)
				Unread (ch);
		}

		public string ReadToWhitespace ()
		{
			var sb = new StringBuilder ();
			int ch = Read ();
			
			for (; ch != -1 && !Char.IsWhiteSpace((char) ch); sb.Append ((char) ch), ch = Read ())
				;
			
			if (ch != -1)
				Unread (ch);
			
			return sb.ToString ();
		}

		public void MarkLocation ()
		{
			if (markedLocation == Location.Unknown)
				markedLocation = new Location (location);
			else
				markedLocation.CopyFrom (location);
		}

		public void RestoreLocation ()
		{
			if (markedLocation != Location.Unknown)
				location.CopyFrom (markedLocation);
		}
	}
}
