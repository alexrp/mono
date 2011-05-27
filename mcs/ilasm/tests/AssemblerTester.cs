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
using System.Diagnostics;
using System.IO;
using Mono.Cecil;
using NUnit.Framework;

namespace Mono.ILAsm.Tests {
	public abstract class AssemblerTester {
		protected sealed class Assembler {
			private readonly Process process;
			private string output_file_name;
			private bool dll = true;
			private string arguments = string.Empty;
			private bool silence = true;
			private bool expect_error;
			
			public Assembler (Process process)
			{
				this.process = process;
			}
			
			public Assembler Input (params string[] fileNames)
			{
				if (output_file_name == null) {
					var name = fileNames [0];
					var ext_idx = name.LastIndexOf ('.');
					
					if (ext_idx == -1)
						ext_idx = name.Length;
					
					output_file_name = name.Substring (0, ext_idx);
				}
				
				foreach (var file in fileNames)
					arguments += "../../tests/" + file;
				
				return this;
			}
			
			public Assembler Dll ()
			{
				dll = true;
				return this;
			}
			
			public Assembler Exe ()
			{
				dll = false;
				return this;
			}
			
			public Assembler Output (string fileName)
			{
				output_file_name = fileName;
				arguments += string.Format (" /output:{0}", fileName);
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
				
				arguments += " " + arg + argument;
				return this;
			}
			
			public Assembler Argument (string argument)
			{
				return Argument (argument, ArgumentType.Slash);
			}
			
			public Assembler Argument (string argument, ArgumentType type, string value)
			{
				Argument (argument, type);
				arguments += ":" + value;
				return this;
			}
			
			public Assembler Argument (string argument, string value)
			{
				return Argument (argument, ArgumentType.Slash, value);
			}
			
			public Assembler Mute ()
			{
				silence = true;
				return this;
			}
			
			public Assembler Unmute ()
			{
				silence = false;
				return this;
			}
			
			public Assembler ExpectError ()
			{
				expect_error = true;
				return this;
			}
			
			public AssemblerOutput Run ()
			{
				if (silence)
					process.StartInfo.RedirectStandardOutput = true;
				
				if (expect_error)
					process.StartInfo.RedirectStandardError = true;
				
				arguments += dll ? " /dll" : " /exe";
				
				process.StartInfo.Arguments = arguments;
				process.Start ();
				process.WaitForExit ();
				return new AssemblerOutput (output_file_name, dll, (AssemblerResult) process.ExitCode);
			}
		}
		
		protected sealed class AssemblerOutput {
			public AssemblerOutput (string fileName, bool dll, AssemblerResult result)
			{
				Result = result;
				file_name = fileName;
				target_dll = dll;
			}
			
			public AssemblerResult Result { get; private set; }
			
			private readonly string file_name;
			
			private readonly bool target_dll;
			
			public AssemblerOutput Expect (AssemblerResult result)
			{
				Assert.AreEqual (result, Result);
				return this;
			}
			
			public AssembledModule GetModule ()
			{
				return new AssembledModule ("../../tests/" + file_name + (target_dll ? ".dll" : ".exe"));
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
		
		protected enum AssemblerResult : byte {
			Success = 0,
			Error = 1,
			Abort = 2,
		}
		
		protected Assembler OpenILAsm ()
		{
			var startInfo = new ProcessStartInfo ("ilasm.exe");
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = false;
			
			var proc = new Process ();
			proc.StartInfo = startInfo;
			return new Assembler (proc);
		}
	}
}
