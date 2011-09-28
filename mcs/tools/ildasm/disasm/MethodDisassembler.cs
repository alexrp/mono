// 
// MethodDisassembler.cs
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
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mono.ILDasm {
	internal sealed class MethodDisassembler : DisassemblerBase {
		readonly MethodDefinition method;
		readonly ModuleDisassembler module;
		
		public MethodDisassembler (ModuleDisassembler module, MethodDefinition method)
			: base (module.Writer)
		{
			this.module = module;
			this.method = method;
		}
		
		public void Disassemble ()
		{
			if (module.ShowMetadataTokens)
				Writer.WriteIndentedLine ("// MDT: {0}", method.MetadataToken);
			
			Writer.WriteIndented (".method ");
			
			if (method.IsPublic)
				Writer.Write ("public ");
			
			if (method.IsPrivate)
				Writer.Write ("private ");
			
			if (method.IsFamily)
				Writer.Write ("family ");
			
			if (method.IsAssembly)
				Writer.Write ("assembly ");
			
			if (method.IsFamilyAndAssembly)
				Writer.Write ("famandassem ");
			
			if (method.IsFamilyOrAssembly)
				Writer.Write ("famorassem ");
			
			if (method.IsStatic)
				Writer.Write ("static ");
			
			if (method.IsFinal)
				Writer.Write ("final ");
			
			if (method.IsVirtual)
				Writer.Write ("virtual ");
			
			if (method.IsAbstract)
				Writer.Write ("abstract ");
			
			if (method.IsHideBySig)
				Writer.Write ("hidebysig ");
			
			if (method.IsNewSlot)
				Writer.Write ("newslot ");
			
			if (method.HasSecurity)
				Writer.Write ("reqsecobj ");
			
			if (method.IsRuntimeSpecialName)
				Writer.Write ("rtspecialname ");
			
			if (method.IsSpecialName)
				Writer.Write ("specialname ");
			
			if (method.IsCheckAccessOnOverride)
				Writer.Write ("strict ");
			
			if (method.IsCompilerControlled)
				Writer.Write ("compilercontrolled ");
			
			if (method.IsUnmanagedExport)
				Writer.Write ("unmanagedexp ");
			
			// TODO: Write P/Invoke info.
			
			if (method.HasThis)
				Writer.Write ("instance ");
			
			if (method.ExplicitThis)
				Writer.Write ("explicit ");
			
			Writer.Write ("{0} ", Stringize (method.CallingConvention));
			
			// TODO: Write return value attributes (Cecil limitation).
			
			Writer.Write ("{0} ", Stringize (method.ReturnType));
			
			// TODO: Write marshal clause.
			
			Writer.Write ("{0}", Escape (method.Name));
			
			if (method.HasGenericParameters) {
				Writer.Write ("<");
				
				for (var i = 0; i < method.GenericParameters.Count; i++) {
					var gp = method.GenericParameters [i];
					
					if (gp.IsCovariant)
						Writer.Write ("+ ");
					
					if (gp.IsContravariant)
						Writer.Write ("- ");
					
					if (gp.HasDefaultConstructorConstraint)
						Writer.Write (".ctor ");
					
					if (gp.HasNotNullableValueTypeConstraint)
						Writer.Write ("valuetype ");
					
					if (gp.HasReferenceTypeConstraint)
						Writer.Write ("class ");
					
					if (gp.HasConstraints) {
						Writer.Write ("(");
						
						for (var j = 0; j < gp.Constraints.Count; j++) {
							Writer.Write (Stringize (gp.Constraints [j]));
							
							if (j != gp.Constraints.Count - 1)
								Writer.Write (", ");
						}
						
						Writer.Write (")");
					}
					
					Writer.Write (Escape (gp.Name));
					
					if (i != method.GenericParameters.Count - 1)
						Writer.Write (", ");
				}
				
				Writer.Write (">");
			}
			
			Writer.Write (" (");
			
			for (var i = 0; i < method.Parameters.Count; i++) {
				var param = method.Parameters [i];
				Writer.Write ("{0}{1}", Stringize (param.ParameterType),
					EscapeOrEmpty (param.Name));
				
				if (i != method.Parameters.Count - 1)
					Writer.Write (", ");
			}
			
			Writer.WriteLine (")");
			
			if (method.ImplAttributes != 0) {
				Writer.Indent ();
				Writer.WriteIndented (string.Empty);
				
				if (method.IsNative)
					Writer.Write ("native ");
				
				if (method.IsIL)
					Writer.Write ("cil ");
				
				if (method.IsManaged)
					Writer.Write ("managed ");
				
				if (method.IsUnmanaged)
					Writer.Write ("unmanaged ");
				
				if (method.IsForwardRef)
					Writer.Write ("forwardref ");
				
				if (method.IsPreserveSig)
					Writer.Write ("preservesig ");
				
				if (method.IsRuntime)
					Writer.Write ("runtime ");
				
				if (method.IsInternalCall)
					Writer.Write ("internalcall ");
				
				if (method.IsSynchronized)
					Writer.Write ("synchronized ");
				
				if (method.NoInlining)
					Writer.Write ("noinlining ");
				
				if (method.NoOptimization)
					Writer.Write ("nooptimization ");
				
				Writer.WriteLine ();
				Writer.Dedent ();
			}
			
			Writer.OpenBracket ();
			
			WriteEntryPoint ();
			WriteMaxStack ();
			WriteLocals ();
			WriteInstructions ();
			
			Writer.CloseBracket ();
			
			Writer.WriteLine ();
		}
		
		void WriteMaxStack ()
		{
			if (method.Body != null)
				Writer.WriteIndentedLine (".maxstack {0}", method.Body.MaxStackSize);
		}
		
		void WriteLocals ()
		{
			if (method.Body == null || !method.Body.HasVariables)
				return;
			
			Writer.WriteIndented (".locals{0} (", method.Body.InitLocals ?
				" init" : string.Empty);
			
			for (var i = 0; i < method.Body.Variables.Count; i++) {
				var local = method.Body.Variables [i];
				Writer.Write ("{0}{1}", Stringize (local.VariableType),
					EscapeOrEmpty (local.Name));
				
				if (i != method.Body.Variables.Count - 1)
					Writer.Write (", ");
			}
			
			Writer.WriteLine (")");
		}
		
		void WriteEntryPoint ()
		{
			if (method == method.Module.EntryPoint)
				Writer.WriteIndentedLine (".entrypoint");
		}
		
		void WriteInstructions ()
		{
			if (method.Body == null || method.Body.Instructions.Count == 0)
				return;
			
			Writer.WriteLine ();
			
			var ehCount = 0;
			var filterCount = 0;
			
			foreach (var instr in method.Body.Instructions) {
				if (!module.RawExceptionHandlers) {
					foreach (var ex in method.Body.ExceptionHandlers) {
						if (instr == ex.TryStart) {
							ehCount++;
							
							Writer.WriteIndentedLine (".try");
							Writer.OpenBracket ();
						}
						
						if (instr == ex.HandlerStart) {
							if (ehCount > 0) {
								ehCount--;
								
								Writer.CloseBracket ();
							}
							
							switch (ex.HandlerType) {
							case ExceptionHandlerType.Catch:
								Writer.WriteIndentedLine ("catch {0}", Stringize (ex.CatchType));
								break;
							case ExceptionHandlerType.Fault:
								Writer.WriteIndentedLine ("fault");
								break;
							case ExceptionHandlerType.Filter:
								// The handler clause of a filter does not have any
								// keyword associated with it.
								if (filterCount > 0) {
									filterCount--;
									
									Writer.CloseBracket ();
								}
								break;
							case ExceptionHandlerType.Finally:
								Writer.WriteIndentedLine ("finally");
								break;
							}
							
							Writer.OpenBracket ();
						}
						
						if (instr == ex.FilterStart) {
							filterCount++;
							
							Writer.WriteIndentedLine ("filter");
							Writer.OpenBracket ();
						}
						
						if (instr == ex.HandlerEnd)
							Writer.CloseBracket ();
					}
				}
				
				Writer.WriteIndented (Stringize (instr));
				
				if (instr.Operand != null) {
					Writer.Write (" ");
					
					var arg = instr.Operand;
					
					if (arg is Instruction)
						Writer.Write (((Instruction) arg).MakeLabel ());
					else if (arg is Instruction[]) {
						var labels = (Instruction[]) arg;
						
						Writer.Write ("( ");
						
						for (var i = 0; i < labels.Length; i++) {
							Writer.Write (labels [i].MakeLabel ());
							
							if (i != labels.Length - 1)
								Writer.Write (", ");
						}
						
						Writer.Write (" )");
					} else if (arg is ParameterDefinition) {
						var paramArg = (ParameterDefinition) arg;
						
						if (paramArg.Name == string.Empty)
							Writer.Write (paramArg.Index.ToString ());
						else
							Writer.Write (Escape (((ParameterDefinition) arg).Name));
					} else if (arg is VariableDefinition) {
						var varArg = (VariableDefinition) arg;
						
						if (varArg.Name == string.Empty)
							Writer.Write (varArg.Index.ToString ());
						else
							Writer.Write (Escape (((VariableDefinition) arg).Name));
					} else if (arg is TypeReference)
						Writer.Write (Stringize ((TypeReference) arg));
					else if (arg is MethodReference) {
						if (instr.OpCode.OperandType == OperandType.InlineTok)
							Writer.Write ("method ");
						
						Writer.Write (Stringize ((MethodReference) arg));
					} else if (arg is FieldReference) {
						if (instr.OpCode.OperandType == OperandType.InlineTok)
							Writer.Write ("field ");
						
						Writer.Write (Stringize ((FieldReference) arg));
					} else if (arg is string) {
						Writer.Write ("\"{0}\"", EscapeQString ((string) arg));
					} else // Integers and floats.
						Writer.Write (arg.ToString ());
				}
				
				Writer.WriteLine ();
				
				if (!module.RawExceptionHandlers)
					foreach (var ex in method.Body.ExceptionHandlers)
						if (instr == method.Body.Instructions [method.Body.Instructions.Count - 1])
							for (var i = 0; i < ehCount; i++)
								Writer.CloseBracket ();
			}
			
			if (module.RawExceptionHandlers) {
				if (method.Body.ExceptionHandlers.Count > 0)
					Writer.WriteLine ();
				
				foreach (var ex in method.Body.ExceptionHandlers) {
					Writer.WriteIndented (".try {0} to {1} ", ex.TryStart.MakeLabel (),
						ex.TryEnd.MakeLabel ());
					
					switch (ex.HandlerType) {
					case ExceptionHandlerType.Catch:
						Writer.Write ("catch {0} ", Stringize (ex.CatchType));
						break;
					case ExceptionHandlerType.Fault:
						Writer.Write ("fault ");
						break;
					case ExceptionHandlerType.Filter:
						Writer.Write ("filter {0} ", ex.FilterStart.MakeLabel ());
						break;
					case ExceptionHandlerType.Finally:
						Writer.Write ("finally ");
						break;
					}
					
					Writer.WriteLine ("handler {0} to {1}", ex.HandlerStart.MakeLabel (),
						ex.HandlerEnd.Previous.MakeLabel ());
				}
			}
		}
	}
}
