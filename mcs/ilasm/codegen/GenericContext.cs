// 
// GenericContext.cs
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
	internal sealed class GenericContext {
		public IGenericParameterProvider CurrentProvider { get; set; }
		
		public IGenericParameterProvider CurrentLocalTypeProvider { get; set; }
		
		public IGenericParameterProvider CurrentLocalMethodProvider { get; set; }
		
		public GenericParameter Resolve (string name, GenericParameterType type)
		{
			switch (type)
			{
			case GenericParameterType.Method:
				if (CurrentLocalMethodProvider != null)
					foreach (var gp in CurrentLocalMethodProvider.GenericParameters)
						if (gp.Name == name)
							return gp;
				
				if (CurrentProvider != null && CurrentProvider is MethodReference)
					foreach (var gp in CurrentProvider.GenericParameters)
						if (gp.Name == name)
							return gp;
				
				break;
			case GenericParameterType.Type:
				if (CurrentLocalTypeProvider != null)
					foreach (var gp in CurrentLocalTypeProvider.GenericParameters)
						if (gp.Name == name)
							return gp;
				
				if (CurrentProvider != null && CurrentProvider is TypeReference)
					foreach (var gp in CurrentProvider.GenericParameters)
						if (gp.Name == name)
							return gp;
				break;
			}
			
			return null;
		}
	}
}
