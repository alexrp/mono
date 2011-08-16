// 
// AssemblyTests.cs
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
using Mono.Cecil;
using System.Linq;
using NUnit.Framework;

namespace Mono.ILAsm.Tests {
	[TestFixture]
	public sealed class AssemblyTests : AssemblerTester {
		[Test]
		public void TestEmptyAssemblyDirective ()
		{
			ILAsm ()
				.Input ("assembly/assembly-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Assembly.Name.Name == "assembly001");
		}
		
		[Test]
		public void TestFullAssemblyDirective ()
		{
			ILAsm ()
				.Input ("assembly/assembly-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Assembly.Name.Expect (
					y => y.PublicKey.ListEquals (new byte[] {
						0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
						0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
					}),
					y => y.HashAlgorithm == AssemblyHashAlgorithm.SHA1,
					y => y.Culture == "en-US",
					y => y.Version.Equals (new Version (1, 2, 3, 4))));
		}
		
		[Test]
		public void TestRawLocale ()
		{
			ILAsm ()
				.Input ("assembly/assembly-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Assembly.Name.Culture == "en-US");
		}
		
		[Test]
		public void TestMultipleAssemblyDirectives ()
		{
			ILAsm ()
				.Input ("assembly/assembly-004.il")
				.ExpectError (Error.MultipleAssemblyDirectives)
				.Run ()
				.Expect (ExitCode.Error);
		}
		
		[Test]
		public void TestRetargetableAssembly ()
		{
			ILAsm ()
				.Input ("assembly/assembly-005.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Assembly.Name.IsRetargetable);
		}
		
		[Test]
		public void TestMiscellaneousAssemblyAttributes ()
		{
			ILAsm ()
				.Input ("assembly/assembly-006.il")
				.Run ()
				.Expect (ExitCode.Success);
		}
		
		[Test]
		public void TestInvalidAssemblyHashAlgorithm ()
		{
			ILAsm ()
				.Input ("assembly/assembly-007.il")
				.ExpectWarning (Warning.UnknownHashAlgorithm)
				.Run ()
				.Expect (ExitCode.Success);
		}
		[Test]
		public void TestEmptyAssemblyExternDirective ()
		{
			ILAsm ()
				.Input ("assembly-extern/assembly-extern-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.AssemblyReferences.ContainsOne (
					y => y.Name == "test001"));
		}
		
		[Test]
		public void TestFullAssemblyExternDirective ()
		{
			ILAsm ()
				.Input ("assembly-extern/assembly-extern-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.AssemblyReferences.ContainsOne (
					y => y.PublicKeyToken.ListEquals (new byte[] {
						0x00, 0x05, 0x10, 0x15,
						0x20, 0x25, 0x30, 0x35,
					}),
					y => y.Hash.ListEquals (new byte[] {
						0x19, 0x18, 0x17, 0x16, 0x15, 0x14, 0x13, 0x12, 0x11, 0x10,
						0x09, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
					}),
					y => y.Culture == "en-US",
					y => y.Version.Equals (new Version (1, 2, 3, 4))));
		}
		
		[Test]
		public void TestAssemblyExternDirectiveWithPublicKey ()
		{
			ILAsm ()
				.Input ("assembly-extern/assembly-extern-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.AssemblyReferences.ContainsOne (
					y => y.PublicKey.ListEquals (new byte[] {
						0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
						0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
					})));
		}
		
		[Test]
		public void TestAssemblyExternDirectiveWithPublicKeyAndToken ()
		{
			ILAsm ()
				.Input ("assembly-extern/assembly-extern-004.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.AssemblyReferences.ContainsOne (
					y => y.PublicKey == null,
					y => y.PublicKeyToken.ListEquals (new byte[] {
						0x00, 0x05, 0x10, 0x15,
						0x20, 0x25, 0x30, 0x35,
					})));
		}
		
		[Test]
		public void TestShadowedAssemblyExternDirective ()
		{
			ILAsm ()
				.Input ("assembly-extern/assembly-extern-005.il")
				.ExpectWarning (Warning.AssemblyReferenceIgnored)
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.AssemblyReferences.ContainsOne (
					y => y.Version.Equals (new Version (1, 0, 0, 0))));
		}
		
		[Test]
		public void TestShadowedAssemblyExternAsDirective ()
		{
			ILAsm ()
				.Input ("assembly-extern/assembly-extern-006.il")
				.ExpectWarning (Warning.AssemblyReferenceIgnored)
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.AssemblyReferences.ContainsOne (
					y => y.Version.Equals (new Version (1, 0, 0, 0))));
		}
		
		[Test]
		public void TestLocaleBytes ()
		{
			ILAsm ()
				.Input ("assembly/assembly-008.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Assembly.Name.Culture == "en-US");
		}
	}
}
