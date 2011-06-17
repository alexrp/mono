// 
// ClassTests.cs
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
	public sealed class ClassTests : AssemblerTester {
		[Test]
		public void TestSimpleClassDirective ()
		{
			ILAsm ()
				.Input ("class/class-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.Types.Contains (
					y => y.Name == "test001",
					y => y.BaseType.FullName == "System.Object"));
		}
		
		[Test]
		public void TestValueTypeClassDirective ()
		{
			ILAsm ()
				.Input ("class/class-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test002").IsValueType);
		}
		
		[Test]
		public void TestEnumClassDirective ()
		{
			ILAsm ()
				.Input ("class/class-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test003").IsEnum);
		}
		
		[Test]
		public void TestInterfaceImplementation ()
		{
			ILAsm ()
				.Input ("class/class-004.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test004").Interfaces.Contains (
					y => y.FullName == "System.ICloneable"));
		}
		
		[Test]
		public void TestMultipleInterfaceImplementations ()
		{
			ILAsm ()
				.Input ("class/class-005.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test005").Interfaces.ContainsMany (
					y => y.FullName == "System.ICloneable",
					y => y.FullName == "System.IDisposable"));
		}
		
		[Test]
		public void TestSimpleClassInheritance ()
		{
			ILAsm ()
				.Input ("class/class-006.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test006").BaseType.Name == "test006_base");
		}
		
		[Test]
		public void TestInterfaceClassDirective ()
		{
			ILAsm ()
				.Input ("class/class-007.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test007").IsInterface);
		}
		
		[Test]
		public void TestSimpleInterfaceImplementation ()
		{
			ILAsm ()
				.Input ("class/class-008.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test008").Interfaces.Contains (
					y => y.Name == "test008_if"));
		}
		
		[Test]
		public void TestGenericValueTypeConstraint ()
		{
			ILAsm ()
				.Input ("class-generic/class-generic-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetTypes ().Contains (
					y => y.GenericParameters.Contains (
						z => z.Attributes.HasBitFlag (GenericParameterAttributes.NotNullableValueTypeConstraint))));
		}
		
		[Test]
		public void TestGenericReferenceTypeConstraint ()
		{
			ILAsm ()
				.Input ("class-generic/class-generic-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetTypes ().Contains (
					y => y.GenericParameters.Contains (
						z => z.Attributes.HasBitFlag (GenericParameterAttributes.ReferenceTypeConstraint))));
		}
		
		[Test]
		public void TestGenericConstructorConstraint ()
		{
			ILAsm ()
				.Input ("class-generic/class-generic-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetTypes ().Contains (
					y => y.GenericParameters.Contains (
						z => z.Attributes.HasBitFlag (GenericParameterAttributes.DefaultConstructorConstraint))));
		}
		
		[Test]
		public void TestGenericInterfaceImplementation ()
		{
			ILAsm ()
				.Input ("class-generic/class-generic-004.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("test013").Interfaces.Contains (
					y => y.FullName == "System.IEquatable`1<test013_dummy>"));
		}
		
		[Test]
		public void TestGenericClassDirective ()
		{
			ILAsm ()
				.Input ("class-generic/class-generic-005.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetType ("Pair`2").GenericParameters.ContainsMany (
					z => z.Name == "T1",
					z => z.Name == "T2"));
		}
	}
}
