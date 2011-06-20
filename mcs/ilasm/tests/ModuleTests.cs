// 
// ModuleTests.cs
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
using NUnit.Framework;

namespace Mono.ILAsm.Tests {
	[TestFixture]
	public sealed class ModuleTests : AssemblerTester {
		[Test]
		public void TestModuleDirective ()
		{
			ILAsm ()
				.Input ("module/module-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Name == "test001");
		}
		
		[Test]
		public void TestMultipleModuleDirectives ()
		{
			ILAsm ()
				.Input ("module/module-002.il")
				.ExpectWarning (Warning.ModuleDirectiveIgnored)
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Name == "test002");
		}
		[Test]
		public void TestModuleExternDirective ()
		{
			ILAsm ()
				.Input ("module-extern/module-extern-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.ModuleReferences.ContainsOne (
					y => y.Name == "test001.dll"));
		}
		
		[Test]
		public void TestDuplicateModuleExternDirective ()
		{
			ILAsm ()
				.Input ("module-extern/module-extern-002.il")
				.ExpectWarning (Warning.ModuleReferenceIgnored)
				.Run ()
				.Expect (ExitCode.Success);
		}
	}
}
