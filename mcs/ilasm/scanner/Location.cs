// Location.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;

namespace Mono.ILAsm {
	public sealed class Location : ICloneable {
		internal int line;
		internal int column;

		public int Line {
			get { return line; }
		}

		public int Column {
			get { return column; }
		}

		public static Location Unknown {
			get { return new Location (-1, -1); }
		}

		public Location ()
		{
			line = 1;
			column = 1;
		}

		public Location (int line, int column)
		{
			this.line = line;
			this.column = column;
		}

		public Location (Location that)
		{
			this.line = that.line;
			this.column = that.column;
		}

		public void NewLine ()
		{
			++line;
			column = 1;
		}

		public void PreviousLine ()
		{
			--line;
			column = 1;
		}

		public void NextColumn ()
		{
			++column;
		}

		public void PreviousColumn ()
		{
			--column;
		}

		public void CopyFrom (Location other)
		{
			this.line = other.line;
			this.column = other.column;
		}

		public virtual object Clone ()
		{
			return new Location (this);
		}

		public override string ToString ()
		{
			return "line (" + line + ") column (" + column + ")";
		}
	}
}
