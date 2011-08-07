// 
// TypeDisassembler.cs
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
using System.IO;
using Mono.Cecil;

namespace Mono.ILDasm {
	internal sealed class TypeDisassembler : DisassemblerBase {
		readonly TypeDefinition type;
		
		readonly ModuleDisassembler module;
		
		public TypeDisassembler (ModuleDisassembler module, TypeDefinition type)
			: base (module.Writer)
		{
			this.module = module;
			this.type = type;
		}
		
		public void Disassemble ()
		{
			if (module.ShowMetadataTokens)
				Writer.WriteIndentedLine ("// MDT: {0}", type.MetadataToken);
			
			Writer.WriteIndented (".class ");
			
			// Some of these could be simplified since several of the
			// values are 0, but this could change in the future, so we
			// just check each flag explicitly for maintainability.
			if (type.IsNotPublic)
				Writer.Write ("private ");
			
			if (type.IsPublic)
				Writer.Write ("public ");
			
			if (type.IsNestedPublic)
				Writer.Write ("nested public ");
			
			if (type.IsNestedPrivate)
				Writer.Write ("nested private ");
			
			if (type.IsNestedFamily)
				Writer.Write ("nested family ");
			
			if (type.IsNestedAssembly)
				Writer.Write ("nested assembly ");
			
			if (type.IsNestedFamilyAndAssembly)
				Writer.Write ("nested famandassem ");
			
			if (type.IsNestedFamilyOrAssembly)
				Writer.Write ("nested famorassem ");
			
			if (type.IsInterface)
				Writer.Write ("interface ");
			
			if (type.IsSealed)
				Writer.Write ("sealed ");
			
			if (type.IsAbstract)
				Writer.Write ("abstract ");
			
			if (type.IsAutoLayout)
				Writer.Write ("auto ");
			
			if (type.IsSequentialLayout)
				Writer.Write ("sequential ");
			
			if (type.IsExplicitLayout)
				Writer.Write ("explicit ");
			
			if (type.IsAnsiClass)
				Writer.Write ("ansi ");
			
			if (type.IsUnicodeClass)
				Writer.Write ("unicode ");
			
			if (type.IsAutoClass)
				Writer.Write ("autochar ");
			
			if (type.IsImport)
				Writer.Write ("import ");
			
			if (type.IsSerializable)
				Writer.Write ("serializable ");
			
			if (type.IsBeforeFieldInit)
				Writer.Write ("beforefieldinit ");
			
			if (type.IsSpecialName)
				Writer.Write ("specialname ");
			
			if (type.IsRuntimeSpecialName)
				Writer.Write ("rtspecialname ");
			
			Writer.Write (Escape (type.GetSanitizedName ()));
			
			if (type.HasGenericParameters) {
				Writer.Write ("<");
				
				for (var i = 0; i < type.GenericParameters.Count; i++) {
					var gp = type.GenericParameters [i];
					
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
					
					if (i != type.GenericParameters.Count - 1)
						Writer.Write (", ");
				}
				
				Writer.Write (">");
			}
			
			Writer.WriteLine ();
			
			if (type.BaseType != null) {
				Writer.Indent ();
				
				Writer.WriteIndentedLine ("extends {0}", Stringize (type.BaseType));
				
				if (type.HasInterfaces) {
					Writer.WriteIndented ("implements ");
					
					for (var i = 0; i < type.Interfaces.Count; i++) {
						Writer.Write (Stringize (type.Interfaces [i]));
						
						if (i != type.Interfaces.Count - 1)
							Writer.Write (", ");
					}
					
					Writer.WriteLine ();
				}
				
				Writer.Dedent ();
			}
			
			Writer.OpenBracket ();
			
			WriteLayoutInfo ();
			WriteNestedTypes ();
			WriteFields ();
			WriteMethods ();
			WriteProperties ();
			WriteEvents ();
			
			Writer.CloseBracket ();
			Writer.WriteLine ();
		}
		
		void WriteLayoutInfo ()
		{
			if (type.ClassSize != -1) {
				Writer.WriteIndentedLine (".size {0}", type.ClassSize);
				Writer.WriteLine ();
			}
			
			if (type.PackingSize != -1) {
				Writer.WriteIndentedLine (".pack {0}", type.PackingSize);
				Writer.WriteLine ();
			}
		}
		
		void WriteNestedTypes ()
		{
			foreach (var nested in type.NestedTypes)
				if (nested.FullName != "<Module>")
					new TypeDisassembler (module, nested).Disassemble ();
		}
		
		void WriteFields ()
		{
		}
		
		void WriteMethods ()
		{
		}
		
		void WriteProperties ()
		{
		}
		
		void WriteEvents ()
		{
		}
	}
}
