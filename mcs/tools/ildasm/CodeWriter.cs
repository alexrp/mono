// 
// CodeWriter.cs
//  
// Author:
//       Alex Rønne Petersen <alex@alexrp.com>
//       Jb Evain (jbevain@gmail.com)
// 
// Copyright (c) 2011 Alex Rønne Petersen, Jb Evain
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Text;

namespace Mono.ILDasm {
	internal sealed class CodeWriter {
		int indent_level;
		
		public string IndentString { get; set; }
		
		public TextWriter Output { get; set; }
		
		public CodeWriter (TextWriter output)
		{
			IndentString = "\t";
			Output = output;
		}
		
		string GetIndent ()
		{
			var sb = new StringBuilder ();
			
			for (var i = 0; i < indent_level; i++)
				sb.Append (IndentString);
			
			return sb.ToString ();
		}
		
		public void Indent ()
		{
			indent_level++;
		}
		
		public void Dedent ()
		{
			indent_level--;
		}
		
		public void Write (string text)
		{
			Output.Write (text);
		}
		
		public void Write (string format, params object[] args)
		{
			Output.Write (format, args);
		}
		
		public void WriteLine ()
		{
			Output.WriteLine ();
		}
		
		public void WriteLine (string text)
		{
			Output.WriteLine (text);
		}
		
		public void WriteLine (string format, params object[] args)
		{
			Output.WriteLine (format, args);
		}
		
		public void WriteIndented (string text)
		{
			Output.Write (GetIndent () + text);
		}
		
		public void WriteIndented (string format, params object[] args)
		{
			Output.Write (GetIndent () + format, args);
		}
		
		public void WriteIndentedLine (string text)
		{
			Output.WriteLine (GetIndent () + text);
		}
		
		public void WriteIndentedLine (string format, params object[] args)
		{
			Output.WriteLine (GetIndent () + format, args);
		}
		
		public void OpenBracket ()
		{
			WriteIndentedLine ("{");
			Indent ();
		}
		
		public void CloseBracket ()
		{
			Dedent ();
			WriteIndentedLine ("}");
		}
	}
}
