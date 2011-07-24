// 
// PropertyTests.cs
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
using NUnit.Framework;

namespace Mono.ILAsm.Tests {
	[TestFixture]
	public class PropertyTests : AssemblerTester {
		[Test]
		public void TestSimpleProperty ()
		{
			ILAsm ()
				.Input ("property/property-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test001_cls").Properties.ContainsOne (
					y => y.Name == "test001",
					y => y.PropertyType.Name == "UInt32"));
		}
		
		/*
		 * FIXME: Cecil assumes that the parameters of the property's methods
		 * are the same as those of the property. We can't currently work
		 * around this, so it needs to be fixed upstream.
		[Test]
		public void TestParametrizedProperty ()
		{
			ILAsm ()
				.Input ("property/property-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test002_cls").Properties.ContainsOne (
					y => y.Parameters.ContainsOne (
						z => z.Name == "val",
						z => z.ParameterType.Name == "UInt32")));
		}
		*/
		
		[Test]
		public void TestPropertyWithAccessors ()
		{
			ILAsm ()
				.Input ("property/property-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test003_cls").Properties.ContainsOne (
					y => y.GetMethod.Name == "get_test003",
					y => y.SetMethod.Name == "set_test003"));
		}
		
		[Test]
		public void TestPropertyWithOtherMethods ()
		{
			ILAsm ()
				.Input ("property/property-004.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test004_cls").Properties.ContainsOne (
					y => y.OtherMethods.ContainsMany (
						z => z.Name == "other_test004_0",
						z => z.Name == "other_test004_1",
						z => z.Name == "other_test004_2")));
		}
	}
}
