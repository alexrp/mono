// 
// Corlib.cs
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

namespace Mono.ILAsm {
	public sealed class Corlib {
		public Corlib (ModuleDefinition module, AssemblyNameReference asmName)
		{
			foreach (var prop in typeof (Corlib).GetProperties ())
				prop.SetValue (this, new TypeReference ("System",
					prop.Name, module, asmName), null);
		}
		
		public TypeReference Object { get; private set; }
		
		public TypeReference ValueType { get; private set; }
		
		public TypeReference Enum { get; private set; }
		
		public TypeReference Boolean { get; private set; }
		
		public TypeReference Char { get; private set; }
		
		public TypeReference String { get; private set; }
		
		public TypeReference Byte { get; private set; }
		
		public TypeReference SByte { get; private set; }
		
		public TypeReference Int16 { get; private set; }
		
		public TypeReference UInt16 { get; private set; }
		
		public TypeReference Int32 { get; private set; }
		
		public TypeReference UInt32 { get; private set; }
		
		public TypeReference Int64 { get; private set; }
		
		public TypeReference UInt64 { get; private set; }
		
		public TypeReference Single { get; private set; }
		
		public TypeReference Double { get; private set; }
		
		public TypeReference Decimal { get; private set; }
		
		public TypeReference IntPtr { get; private set; }
		
		public TypeReference UIntPtr { get; private set; }
		
		public TypeReference TypedReference { get; private set; }
		
		public TypeReference Void { get; private set; }
	}
}
