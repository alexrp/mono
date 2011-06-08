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

namespace Mono.ILAsm.Tests {
	internal static class Extensions {
		public static bool ListEquals<T> (this IList<T> list, IList<T> other)
		{
			if (list == null && other != null)
				return false;
			
			if (list != null && other == null)
				return false;
			
			if (list == null && other == null)
				return true;
			
			if (list.Count != other.Count)
				return false;
			
			for (var i = 0; i < list.Count; i++)
				if (!list [i].Equals (other [i]))
					return false;
			
			return true;
		}
		
		public static bool Contains<T> (this IEnumerable<T> list, params Predicate<T>[] predicates)
		{
			bool flag = false;
			
			foreach (var item in list) {
				foreach (var predicate in predicates)
					if (!(flag = predicate (item)))
						break;
				
				if (flag)
					return true;
			}
			
			return false;
		}
		
		public static bool ContainsMany<T> (this IEnumerable<T> list, params Predicate<T>[] predicates)
		{
			var passes = 0;
			
			foreach (var item in list) {
				foreach (var predicate in predicates) {
					if (predicate (item))
						passes++;
					
					if (passes == predicates.Length)
						return true;
				}
			}
			
			return false;
		}
		
		public static bool HasBitFlag (this Enum value, Enum flag)
		{
			var intValue = ((IConvertible) value).ToUInt64 (null);
			var intFlag = ((IConvertible) flag).ToUInt64 (null);
			
			return (intValue & intFlag) != 0;
		}
	}
}
