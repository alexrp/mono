// 
// ResourceTests.cs
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
using Mono.Cecil;
using NUnit.Framework;

namespace Mono.ILAsm.Tests {
	[TestFixture]
	public class ResourceTests : AssemblerTester {
		[Test]
		public void TestAssemblyExternResource ()
		{
			ILAsm ()
				.Input ("mresource/mresource-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Resources.ContainsOne (
					y => y.Name == "test001",
					y => ((AssemblyLinkedResource) y).Assembly.Name == "mscorlib"));
		}
		
		[Test]
		public void TestFileResource ()
		{
			ILAsm ()
				.Input ("mresource/mresource-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Resources.ContainsOne (
					y => y.Name == "test002_rsc",
					y => ((LinkedResource) y).File == "test002"));
		}
		
		[Test]
		public void TestInsaneDefaultResourceVisibility ()
		{
			ILAsm ()
				.Input ("mresource/mresource-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Resources.ContainsOne (
					y => y.Name == "test003",
					y => !y.IsPublic && !y.IsPrivate));
		}
		
		[Test]
		public void TestEmbeddedResource ()
		{
			ILAsm ()
				.Input ("mresource/mresource-004.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Resources.ContainsOne (
					y => y.Name == "../../tests/mresource/test.txt"));
		}
		
		[Test]
		public void TestAliasedResource ()
		{
			ILAsm ()
				.Input ("mresource/mresource-005.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Resources.ContainsOne (
					y => y.Name == "test005"));
		}
	}
}
