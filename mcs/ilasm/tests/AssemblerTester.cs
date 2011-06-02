// 
// AssemblerTester.cs
//  
// Author:
//       Alex Rønne Petersen <alex@alexrp.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mono.Cecil;
using NUnit.Framework;

namespace Mono.ILAsm.Tests {
	public abstract class AssemblerTester {
		protected sealed class Assembler {
			readonly Driver driver;
			readonly List<string> arguments = new List<string> ();
			Error? expected_error;
			Warning? expected_warning;
			Error? resulting_error;
			Warning? resulting_warning;
			
			public Assembler ()
			{
				driver = new Driver ();
				driver.Target = Target.Dll;
				driver.Output = TextWriter.Null;
				
				Report.Quiet = true;
				Report.Warning += OnWarning;
				Report.Error += OnError;
			}
			
			private void OnWarning (object sender, WarningEventArgs e)
			{
				resulting_warning = e.Warning;
			}
			
			private void OnError (object sender, ErrorEventArgs e)
			{
				resulting_error = e.Error;
			}
			
			public Assembler Input (params string[] fileNames)
			{
				foreach (var file in fileNames)
					arguments.Add ("../../tests/" + file);
				
				return this;
			}
			
			public Assembler Dll ()
			{
				driver.Target = Target.Dll;
				return this;
			}
			
			public Assembler Exe ()
			{
				driver.Target = Target.Exe;
				return this;
			}
			
			public Assembler Output (string fileName)
			{
				driver.OutputFileName = fileName;
				return this;
			}
			
			public Assembler Argument (string argument, ArgumentType type)
			{
				string arg = null;
				switch (type) {
				case ArgumentType.Slash:
					arg = "/";
					break;
				case ArgumentType.Dash:
					arg = "-";
					break;
				case ArgumentType.DoubleDash:
					arg = "--";
					break;
				}
				
				arguments.Add (arg + argument);
				return this;
			}
			
			public Assembler Argument (string argument)
			{
				return Argument (argument, ArgumentType.Slash);
			}
			
			public Assembler Argument (string argument, ArgumentType type, string value)
			{
				Argument (argument + ":" + value, type);
				return this;
			}
			
			public Assembler Argument (string argument, string value)
			{
				return Argument (argument, ArgumentType.Slash, value);
			}
			
			public Assembler Mute ()
			{
				driver.Output = TextWriter.Null;
				Report.Quiet = true;
				
				return this;
			}
			
			public Assembler Unmute ()
			{
				driver.Output = Console.Out;
				Report.Quiet = false;
				
				return this;
			}
			
			public Assembler ExpectError (Error error)
			{
				expected_error = error;
				return this;
			}
			
			public Assembler ExpectWarning (Warning warning)
			{
				expected_warning = warning;
				return this;
			}
			
			public AssemblerOutput Run ()
			{
				if (expected_warning != null)
					Report.WarningOutput = TextWriter.Null;
				
				if (expected_error != null)
					Report.ErrorOutput = TextWriter.Null;
				
				var result = driver.Run (arguments.ToArray ());
				
				// Reset stuff to defaults.
				driver.Output = Console.Out;
				
				Report.Quiet = false;
				Report.Warning -= OnWarning;
				Report.Error -= OnError;
				Report.ErrorOutput = Console.Error;
				Report.WarningOutput = Console.Out;
				
				Assert.AreEqual (expected_warning, resulting_warning);
				Assert.AreEqual (expected_error, resulting_error);
				
				return new AssemblerOutput (driver.OutputFileName, result);
			}
		}
		
		protected sealed class AssemblerOutput {
			public AssemblerOutput (string fileName, ExitCode? result)
			{
				Result = result;
				file_name = fileName;
			}
			
			public ExitCode? Result { get; private set; }
			
			private readonly string file_name;
			
			public AssemblerOutput Expect (ExitCode? result)
			{
				Assert.AreEqual (result, Result);
				return this;
			}
			
			public AssembledModule GetModule ()
			{
				return new AssembledModule (file_name);
			}
		}
		
		public delegate bool ModulePredicate (ModuleDefinition module);
		
		protected sealed class AssembledModule {
			public AssembledModule (string fileName)
			{
				Module = ModuleDefinition.ReadModule (fileName);
			}
			
			public ModuleDefinition Module { get; private set; }
			
			public AssembledModule Expect (ModulePredicate predicate)
			{
				Assert.IsTrue (predicate (Module));
				return this;
			}
		}
		
		protected enum ArgumentType : byte {
			Slash = 0,
			Dash = 1,
			DoubleDash = 2,
		}
		
		protected Assembler ILAsm ()
		{
			return new Assembler ();
		}
	}
}
