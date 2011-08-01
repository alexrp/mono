// 
// MethodTests.cs
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
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Mono.ILAsm.Tests {
	[TestFixture]
	public sealed class MethodTests : AssemblerTester {
		[Test]
		public void TestSimpleMethod ()
		{
			ILAsm ()
				.Input ("method/method-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Name == "test001",
					y => y.ReturnType.Name == "Void"));
		}
		
		[Test]
		public void TestEmptyMethod ()
		{
			ILAsm ()
				.Input ("method/method-002.il")
				.ExpectWarning (Warning.DefaultReturnEmitted)
				.Run ()
				.Expect (ExitCode.Success);
		}
		
		[Test]
		public void TestParametrizedMethod ()
		{
			ILAsm ()
				.Input ("method/method-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Parameters.ContainsMany (
						z => z.Name == "val1" && z.ParameterType.Name == "UInt32",
						z => z.Name == "val2" && z.ParameterType.Name == "Single")));
		}
		
		[Test]
		public void TestGenericMethod ()
		{
			ILAsm ()
				.Input ("method-generic/method-generic-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.GenericParameters.ContainsMany (
						z => z.Name == "T1",
						z => z.Name == "T2")));
		}
		
		/*
		 * FIXME: These tests fail due to Cecil's lack of support for unbound
		 * generic parameters. They can be uncommented when this support has
		 * been added upstream.
		[Test]
		public void TestGenericMethodWithGenericReturnType ()
		{
			ILAsm ()
				.Input ("method-generic/method-generic-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.ReturnType.Name == "T"));
		}
		
		[Test]
		public void TestGenericMethodWithGenericParameterType ()
		{
			ILAsm ()
				.Input ("method-generic/method-generic-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Parameters.ContainsOne (
						z => z.ParameterType.Name == "T")));
		}
		*/
		
		/*
		 * FIXME: This test fails because Cecil forces a calculated stack
		 * size rather than using the one we specify. This must be fixed
		 * upstream.
		[Test]
		public void TestMaxStack ()
		{
			ILAsm ()
				.Input ("method-body/method-body-001.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Body.MaxStackSize == 1024));
		}
		*/
		
		/*
		 * FIXME: Something is wrong with symbol writing/reading somewhere;
		 * we don't correctly get local variable names.
		[Test]
		public void TestMethodLocals ()
		{
			ILAsm ()
				.Input ("method-body/method-body-002.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Body.Variables.ContainsMany (
						z => z.Name == "var1" && z.VariableType.Name == "UInt32",
						z => z.Name == "var2" && z.VariableType.Name == "Double")));
		}
		*/
		
		[Test]
		public void TestMethodLocalsWithInit ()
		{
			ILAsm ()
				.Input ("method-body/method-body-003.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Body.InitLocals));
		}
		
		[Test]
		public void TestMethodWithZeroInit ()
		{
			ILAsm ()
				.Input ("method-body/method-body-004.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Body.InitLocals));
		}
		
		[Test]
		public void TestEntryPointMethod ()
		{
			ILAsm ()
				.Input ("method-body/method-body-005.il")
				.Exe ()
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.EntryPoint.Name == "test005");
		}
		
		[Test]
		public void TestDuplicateEntryPointMethod ()
		{
			ILAsm ()
				.Input ("method-body/method-body-006.il")
				.ExpectError (Error.MultipleEntryPoints)
				.Run ()
				.Expect (ExitCode.Error);
		}
		
		/*
		 * FIXME: See TestMethodLocals note.
		[Test]
		public void TestLocalVariableReference ()
		{
			ILAsm ()
				.Input ("method-body/method-body-007.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Body.Instructions.Contains (
						z => z.Operand is VariableDefinition,
						z => ((VariableDefinition) z.Operand).Name == "var")));
		}
		*/
		
		[Test]
		public void TestNoOperandInstructions ()
		{
			ILAsm ()
				.Input ("method-body/method-body-008.il")
				.Run ()
				.Expect (ExitCode.Success);
		}
		
		[Test]
		public void TestInvalidLocalName ()
		{
			ILAsm ()
				.Input ("method-body/method-body-009.il")
				.ExpectError (Error.InvalidLocal)
				.Run ()
				.Expect (ExitCode.Error);
		}
		
		[Test]
		public void TestInvalidParameterName ()
		{
			ILAsm ()
				.Input ("method-body/method-body-010.il")
				.ExpectError (Error.InvalidParameter)
				.Run ()
				.Expect (ExitCode.Error);
		}
		
		[Test]
		public void TestInvalidLocalIndex ()
		{
			ILAsm ()
				.Input ("method-body/method-body-011.il")
				.ExpectError (Error.InvalidParameter)
				.Run ()
				.Expect (ExitCode.Error);
		}
		
		[Test]
		public void TestInvalidParameterIndex ()
		{
			ILAsm ()
				.Input ("method-body/method-body-012.il")
				.ExpectError (Error.InvalidLocal)
				.Run ()
				.Expect (ExitCode.Error);
		}
		
		[Test]
		public void TestParameterOperand ()
		{
			ILAsm ()
				.Input ("method-body/method-body-013.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Body.Instructions.Contains (
						z => z.Operand is ParameterDefinition,
						z => ((ParameterDefinition) z.Operand).Name == "arg")));
		}
		
		/*
		 * FIXME: See note in TestMethodLocals.
		[Test]
		public void TestLocalOperands ()
		{
			ILAsm ()
				.Input ("method-body/method-body-014.il")
				.Run ()
				.Expect (ExitCode.Success)
				.GetModule ()
				.Expect (x => x.GetModuleType ().Methods.ContainsOne (
					y => y.Body.Instructions.Contains (
						z => z.Operand is ParameterDefinition && ((ParameterDefinition) z.Operand).Name == "var2",
						z => z.Operand is ParameterDefinition && ((ParameterDefinition) z.Operand).Name == "var1")));
		}
		*/
	}
}
