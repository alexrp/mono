// 
// Extensions.cs
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
	internal static class Extensions {
		public static string ToInt32Hex (this Enum val)
		{
			return "0x" + ((IConvertible) val).ToUInt32 (null).ToString ("X8");
		}
		
		public static string ToInt64Hex (this Enum val)
		{
			return "0x" + ((IConvertible) val).ToUInt64 (null).ToString ("X16");
		}
		
		public static string GetFullNestableName (this TypeReference type)
		{
			var s = string.Empty;
			
			if (!string.IsNullOrEmpty (type.Namespace))
				s += type.Namespace + ".";
			
			return s + type.Name;
		}
	}
}
