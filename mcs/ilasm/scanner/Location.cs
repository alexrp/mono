// Location.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;

namespace Mono.ILAsm {
	public sealed class Location {
		public int Line { get; private set; }

		public int Column { get; private set; }

		public static readonly Location Unknown = new Location (-1, -1);

		public Location ()
		{
			Line = 1;
			Column = 1;
		}

		public Location (int line, int column)
		{
			Line = line;
			Column = column;
		}

		public Location (Location that)
		{
			Line = that.Line;
			Column = that.Column;
		}

		public Location NewLine ()
		{
			return new Location (Line + 1, 1);
		}

		public Location PreviousLine ()
		{
			return new Location (Line - 1, 1);
		}

		public Location NextColumn ()
		{
			return new Location (Line, Column + 1);
		}

		public Location PreviousColumn ()
		{
			return new Location (Line, Column - 1);
		}

		public override string ToString ()
		{
			return "line (" + Line + ") column (" + Column + ")";
		}
	}
}
