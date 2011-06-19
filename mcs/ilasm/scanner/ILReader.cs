// ILReader.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Mono.ILAsm {
	internal sealed class ILReader {
		private readonly Stack<char> putback_stack;
		private Location marked_location;

		public Location Location { get; private set; }

		public StreamReader BaseReader { get; private set; }

		public ILReader (StreamReader reader)
		{
			BaseReader = reader;
			Location = new Location ();
			putback_stack = new Stack<char> ();
			marked_location = Location.Unknown;
		}

		private int DoRead ()
		{
			if (putback_stack.Count > 0) 
				return putback_stack.Pop ();

			return BaseReader.Read ();
		}

		private int DoPeek ()
		{
			if (putback_stack.Count > 0)
				return putback_stack.Peek ();

			return BaseReader.Peek ();
		}

		public int Read ()
		{
			var read = DoRead ();
			
			if (read == '\n')
				Location = Location.NewLine ();
			else
				Location = Location.NextColumn ();
			
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
				Location = Location.PreviousLine ();

			Location = Location.PreviousColumn ();
		}

		public void Unread (char[] chars)
		{
			for (var i = chars.Length-1; i >= 0; i--)
				Unread (chars [i]);
		}

		public void Unread (int c)
		{
			Unread ((char) c);
		}

		public void SkipWhitespace ()
		{
			var ch = Read ();
			
			for (; ch != -1 && char.IsWhiteSpace((char) ch); ch = Read ())
				;
			
			if (ch != -1)
				Unread (ch);
		}

		public string ReadToWhitespace ()
		{
			var sb = new StringBuilder ();
			var ch = Read ();
			
			while (ch != -1 && !char.IsWhiteSpace ((char) ch))
			{
				sb.Append ((char) ch);
				ch = Read ();
			}
			
			if (ch != -1)
				Unread (ch);
			
			return sb.ToString ();
		}

		public void MarkLocation ()
		{
			marked_location = new Location (Location);
		}

		public void RestoreLocation ()
		{
			if (marked_location != Location.Unknown)
				Location = new Location (marked_location);
		}
	}
}
