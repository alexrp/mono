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
using Mono.Collections.Generic;

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
			
			return (intValue & intFlag) == intFlag;
		}
		
		public static bool HasAnyBitFlag (this Enum value, Enum flags)
		{
			var intValue = ((IConvertible) value).ToUInt64 (null);
			var intFlags = ((IConvertible) flags).ToUInt64 (null);
			
			return (intValue & intFlags) != 0;
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
		
		public static IEnumerable<T> QueueReverse<T> (this IEnumerable<T> self)
		{
			// This could probably be done in a more optimal manner. However,
			// this is prettier and easier to read. Sue me.
			var q = new Queue<T> ();
			
			foreach (var item in self)
				q.Enqueue (item);
			
			return q;
		}
		
		public static void NullResize<T> (this Collection<T> col, int newSize)
			where T : class
		{
			while (col.Count != newSize)
				col.Add (null);
		}
	}
}
