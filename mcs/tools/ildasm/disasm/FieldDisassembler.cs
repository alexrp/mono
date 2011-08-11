// 
// FieldDisassembler.cs
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

namespace Mono.ILDasm {
	internal sealed class FieldDisassembler : DisassemblerBase {
		readonly FieldDefinition field;
		
		public FieldDisassembler (ModuleDisassembler module, FieldDefinition field)
			: base (module.Writer)
		{
			this.field = field;
		}
		
		public void Disassemble ()
		{
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
			
			// TODO: Write marshal clause.
			
			Writer.Write ("{0} ", Stringize (field.FieldType));
			Writer.Write (Escape (field.Name));
			
			// NOTE: We can't write the 'at' clause because we
			// don't support .data declarations in the first place.
			
			if (field.HasConstant) {
				Writer.Write (" = ");
				var val = field.Constant;
				
				if (val is string)
					// TODO: This seems broken.
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
			Writer.WriteLine ();
		}
	}
}
