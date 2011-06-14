// 
// Error.cs
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
	public enum Error : short {
		/// <summary>
		/// Something went wrong in the assembler.
		/// </summary>
		InternalError = 0,
		/// <summary>
		/// An input file could not be found.
		/// </summary>
		FileNotFound = 1,
		/// <summary>
		/// A grammatical error was encountered in the source input.
		/// </summary>
		SyntaxError = 2,
		/// <summary>
		/// No entry point found in an executable target.
		/// </summary>
		NoEntryPoint = 3,
		/// <summary>
		/// Multiple entry points were declared in an executable target.
		/// </summary>
		MultipleEntryPoints = 4,
		/// <summary>
		/// Multiple .assembly directives were encountered.
		/// </summary>
		MultipleAssemblyDirectives = 5,
		/// <summary>
		/// Assembly signing failed.
		/// </summary>
		SigningFailed = 6,
		/// <summary>
		/// Happens if an image base is not 0x10000-aligned.
		/// </summary>
		InvalidImageBase = 7,
		/// <summary>
		/// Happens if a file alignment is not a power of two and/or is not
		/// between 0x200 and 0x10000.
		/// </summary>
		InvalidFileAlignment = 8,
		/// <summary>
		/// Happens if a module is referenced in a signature but has not
		/// been referenced with a .module extern directive.
		/// </summary>
		UndeclaredModuleReference = 9,
		/// <summary>
		/// Happens if an instance field has an at clause (this is only valid
		/// for static fields).
		/// </summary>
		InstanceFieldWithDataLocation = 10,
	}
}
