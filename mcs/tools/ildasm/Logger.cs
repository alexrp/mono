// 
// Logger.cs
//  
// Author:
//       Alex Rønne Petersen <alex@alexrp.com>
// 
// Copyright (c) 2011 Alex Rønne Petersen
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

namespace Mono.ILDasm {
	internal static class Logger {
		public static void Info (string format, params object[] args)
		{
			Write (null, format, args);
		}
		
		public static void Warning (string format, params object[] args)
		{
			Write (ConsoleColor.Yellow, "Warning: " + format, args);
		}
		
		public static void Error (string format, params object[] args)
		{
			Write (ConsoleColor.Red, "Error: " + format, args);
		}
		
		public static void Fatal (string format, params object[] args)
		{
			Error (format, args);
			Environment.Exit ((int) ExitCode.Abort);
		}
		
		static void Write (ConsoleColor? color, string format, params object[] args)
		{
			if (color != null)
				Console.ForegroundColor = (ConsoleColor) color;
			
			Console.WriteLine (format, args);
			
			Console.ResetColor ();
		}
	}
}
