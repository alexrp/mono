// 
// EventTests.cs
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
	public class EventTests : AssemblerTester {
		[Test]
		public void TestMissingEventAccessors ()
		{
			ILAsm ()
				.Input ("event/event-001.il")
				.ExpectError (Error.MissingEventAccessors)
				.Run ()
				.Expect (ExitCode.Error);
		}
		
		[Test]
		public void TestSimpleEvent ()
		{
			ILAsm ()
				.Input ("event/event-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test002_cls").Events.ContainsOne (
					y => y.Name == "test002",
					y => y.AddMethod.Name == "add_test002",
					y => y.RemoveMethod.Name == "remove_test002"));
		}
		
		[Test]
		public void TestEventWithFireMethod ()
		{
			ILAsm ()
				.Input ("event/event-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test003_cls").Events.ContainsOne (
					y => y.InvokeMethod.Name == "fire_test003"));
		}
		
		[Test]
		public void TestEventWithOtherMethods ()
		{
			ILAsm ()
				.Input ("event/event-004.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test004_cls").Events.ContainsOne (
					y => y.OtherMethods.ContainsMany (
						z => z.Name == "other_test004_0",
						z => z.Name == "other_test004_1",
						z => z.Name == "other_test004_2")));
		}
	}
}
