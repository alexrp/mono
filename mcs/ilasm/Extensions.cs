// 
// Extensions.cs
//  
// Author:
//       Alex Rønne Petersen <alex@alexrp.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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
using Mono.Cecil;

namespace Mono.ILAsm {
	public static class Extensions {
		public static TValue TryGet<TKey, TValue> (this IDictionary<TKey, TValue> dict,
			TKey key)
			where TValue : class
		{
			TValue value;
			if (dict.TryGetValue (key, out value))
				return value;
			
			return null;
		}
		
		public static bool HasBitFlag (this Enum value, Enum flag)
		{
			var intValue = ((IConvertible) value).ToUInt64 (null);
			var intFlag = ((IConvertible) flag).ToUInt64 (null);
			
			return (intValue & intFlag) != 0;
		}
		
		public static TypeDefinition GetModuleType (this ModuleDefinition module)
		{
			return module.GetType ("<Module>");
		}
		
		public static T[] Inflate<T> (this T[] arr, int addedSize)
		{
			Array.Resize (ref arr, arr.Length + addedSize);
			return arr;
		}
	}
}
