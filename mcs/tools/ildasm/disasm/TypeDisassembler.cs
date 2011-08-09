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
				
				if (type.BaseType != null)
					if (!(type.BaseType.Scope.IsCorlib () &&
						type.BaseType.FullName == "System.Object"))
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
				new TypeDisassembler (module, nested).Disassemble ();
		}
		
		void WriteFields ()
		{
			if (!type.HasFields)
				return;
			
			foreach (var field in type.Fields) {
				Writer.WriteIndented (".field ");
				
				if (field.IsPublic)
					Writer.Write ("public ");
				
				if (field.IsPrivate)
					Writer.Write ("private ");
				
				if (field.IsFamily)
					Writer.Write ("family ");
				
				if (field.IsAssembly)
					Writer.Write ("assembly ");
				
				if (field.IsFamilyAndAssembly)
					Writer.Write ("famandassem ");
				
				if (field.IsFamilyOrAssembly)
					Writer.Write ("famorassem ");
				
				if (field.IsStatic)
					Writer.Write ("static ");
				
				if (field.IsInitOnly)
					Writer.Write ("initonly ");
				
				if (field.IsRuntimeSpecialName)
					Writer.Write ("rtspecialname ");
				
				if (field.IsSpecialName)
					Writer.Write ("specialname ");
				
				if (field.IsLiteral)
					Writer.Write ("literal ");
				
				if (field.IsNotSerialized)
					Writer.Write ("notserialized ");
				
				if (field.Offset != -1)
					Writer.Write ("[{0}] ", field.Offset);
				
				Writer.Write ("{0} ", Stringize (field.FieldType));
				Writer.Write (Escape (field.Name));
				
				// NOTE: We can't write the 'at' clause because we
				// don't support .data declarations in the first place.
				
				if (field.HasConstant) {
					Writer.Write (" = ");
					var val = field.Constant;
					
					if (val is string)
						Writer.Write ("\"{0}\"", val);
					else if (val is bool)
						Writer.Write ("bool ({0})", (bool) val ? "true" : "false");
					else if (val is char)
						Writer.Write ("char ({0})", (int) (char) val);
					else if (val is float)
						Writer.Write ("float32 ({0})", val);
					else if (val is double)
						Writer.Write ("float64 ({0})", val);
					else if (val is byte)
						Writer.Write ("uint8 ({0})", val);
					else if (val is sbyte)
						Writer.Write ("int8 ({0})", val);
					else if (val is short)
						Writer.Write ("int16 ({0})", val);
					else if (val is ushort)
						Writer.Write ("uint16 ({0})", val);
					else if (val is int)
						Writer.Write ("int32 ({0})", val);
					else if (val is uint)
						Writer.Write ("uint32 ({0})", val);
					else if (val is long)
						Writer.Write ("int64 ({0})", val);
					else if (val is ulong)
						Writer.Write ("uint64 ({0})", val);
					else if (val is byte[])
						Writer.Write ("bytearray ", ToByteList ((byte[]) val));
					else
						Writer.Write ("nullref");
				}
				
				Writer.WriteLine ();
			}
			
			Writer.WriteLine ();
		}
		
		void WriteMethods ()
		{
			if (!type.HasMethods)
				return;
			
			Writer.WriteLine ();
		}
		
		void WriteProperties ()
		{
			if (!type.HasProperties)
				return;
			
			foreach (var prop in type.Properties) {
				if (module.ShowMetadataTokens)
					Writer.WriteIndentedLine ("// MDT: {0}", prop.MetadataToken);
				
				Writer.WriteIndented (".property ");
				
				if (prop.HasThis)
					Writer.Write ("instance ");
				
				// TODO: Write calling convention when Cecil supports it.
				
				if (prop.IsRuntimeSpecialName)
					Writer.Write ("rtspecialname ");
				
				if (prop.IsSpecialName)
					Writer.Write ("specialname ");
				
				Writer.Write ("{0} ", Stringize (prop.PropertyType));
				Writer.Write ("{0} ", Escape (prop.Name));
				
				Writer.Write ("(");
				
				for (var i = 0; i < prop.Parameters.Count; i++) {
					var param = prop.Parameters [i];
					Writer.Write ("{0} {1}", Stringize (param.ParameterType),
						Escape (param.Name));
					
					if (i != prop.Parameters.Count - 1)
						Writer.Write (", ");
				}
				
				Writer.WriteLine (")");
				
				Writer.OpenBracket ();
				
				if (prop.GetMethod != null)
					Writer.WriteIndentedLine (".get {0}",
						Stringize (prop.GetMethod));
				
				if (prop.SetMethod != null)
					Writer.WriteIndentedLine (".set {0}",
						Stringize (prop.SetMethod));
				
				foreach (var other in prop.OtherMethods)
					Writer.WriteIndentedLine (".other {0}",
						Stringize (other));
				
				Writer.CloseBracket ();
			}
			
			Writer.WriteLine ();
		}
		
		void WriteEvents ()
		{
			if (!type.HasEvents)
				return;
			
			foreach (var evnt in type.Events) {
				if (module.ShowMetadataTokens)
					Writer.WriteIndentedLine ("// MDT: {0}", evnt.MetadataToken);
				
				Writer.WriteIndented (".event ");
				
				if (evnt.IsRuntimeSpecialName)
					Writer.Write ("rtspecialname ");
				
				if (evnt.IsSpecialName)
					Writer.Write ("specialname ");
				
				Writer.Write ("{0} ", Stringize (evnt.EventType));
				Writer.WriteLine (Escape (evnt.Name));
				
				Writer.OpenBracket ();
				
				if (evnt.AddMethod != null)
					Writer.WriteIndentedLine (".addon {0}",
						Stringize (evnt.AddMethod));
				
				if (evnt.RemoveMethod != null)
					Writer.WriteIndentedLine (".removeon {0}",
						Stringize (evnt.RemoveMethod));
				
				if (evnt.InvokeMethod != null)
					Writer.WriteIndentedLine (".fire {0}",
						Stringize (evnt.InvokeMethod));
				
				foreach (var other in evnt.OtherMethods)
					Writer.WriteIndentedLine (".other {0}",
						Stringize (other));
				
				Writer.CloseBracket ();
			}
			
			Writer.WriteLine ();
		}
	}
}
