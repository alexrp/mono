// 
// Warning.cs
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

namespace Mono.ILAsm {
	public enum Warning : short {
		/// <summary>
		/// Something might have gone wrong in the assembler.
		/// </summary>
		InternalWarning = 0,
		/// <summary>
		/// A .module directive was ignored (usually because one was already
		/// encountered).
		/// </summary>
		ModuleDirectiveIgnored = 1,
		/// <summary>
		/// A .assembly extern directive was ignored (usually happens when one
		/// directive shadows another by name).
		/// </summary>
		AssemblyReferenceIgnored = 2,
		/// <summary>
		/// Happens when an assembly is indirectly referenced but hasn't been
		/// declared earlier. ILAsm will attempt to resolve it in the GAC.
		/// </summary>
		AutoResolvingAssembly = 3,
		/// <summary>
		/// Happens if automatic resolution of an indirectly referenced assembly
		/// fails.
		/// </summary>
		AutoResolutionFailed = 4,
		/// <summary>
		/// Happens when a .module extern directive is ignored (usually happens
		/// when the same module is referenced multiple times).
		/// </summary>
		ModuleReferenceIgnored = 5,
		/// <summary>
		/// Happens when a .namespace directive is encountered.
		/// </summary>
		LegacyNamespaceSyntax = 6,
		/// <summary>
		/// A .hash algorithm directive had an unrecognized value.
		/// </summary>
		UnknownHashAlgorithm = 7,
		/// <summary>
		/// A module-global field was not marked as static.
		/// </summary>
		GlobalFieldMadeStatic = 8,
		/// <summary>
		/// A module-global field had an offset specified; ignored.
		/// </summary>
		GlobalFieldOffsetIgnored = 9,
	}
}
