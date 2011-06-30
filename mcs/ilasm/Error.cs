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
		/// <summary>
		/// An unsupported variant type was used in a safe array marshaling
		/// specification.
		/// </summary>
		UnsupportedVariantType = 11,
		/// <summary>
		/// An unsupported native type was used in a marshaling signature.
		/// </summary>
		UnsupportedNativeType = 12,
		/// <summary>
		/// Happens when a field's at clause refers to a data constant that
		/// has not been defined.
		/// </summary>
		InvalidDataConstantLabel = 13,
		/// <summary>
		/// Happens if an assembly is referenced in a signature but has not
		/// been referenced with a .assembly extern directive.
		/// </summary>
		UndeclaredAssemblyReference = 14,
		/// <summary>
		/// A generic parameter was referenced by ordinal or name outside of
		/// a type definition scope.
		/// </summary>
		GenericParameterAccessOutsideType = 15,
		/// <summary>
		/// A generic parameter was referenced by ordinal or name outside of
		/// a method definition scope.
		/// </summary>
		GenericParameterAccessOutsideMethod = 16,
		/// <summary>
		/// Happens when a generic parameter ordinal is out of range.
		/// </summary>
		GenericParameterOrdinalOutOfRange = 17,
		/// <summary>
		/// Happens when a generic parameter name does not exist.
		/// </summary>
		GenericParameterNameInvalid = 18,
		/// <summary>
		/// Happens when a token in an mdtoken clause is invalid.
		/// </summary>
		InvalidMetadataToken = 19,
		/// <summary>
		/// Happens when an undefined type is referenced within the current
		/// module's scope.
		/// </summary>
		UndefinedTypeReference = 20,
		/// <summary>
		/// A property accessor was invalid (i.e. not in the current module).
		/// </summary>
		InvalidPropertyMethod = 21,
		/// <summary>
		/// An event accessor was invalid (i.e. not in the current module).
		/// </summary>
		InvalidEventMethod = 22,
		/// <summary>
		/// One of the .this, .base, or .nester directives were used outside
		/// of the scope of a type definition.
		/// </summary>
		RelativeTypeReferenceOutsideTypeDefinition = 23,
		/// <summary>
		/// Happens when the .base directive is used in a type definition that
		/// has no base type.
		/// </summary>
		NoBaseType = 24,
		/// <summary>
		/// Happens when the .nester directive is used in a type definition that
		/// is not nested.
		/// </summary>
		NoDeclaringType = 25,
		/// <summary>
		/// Can happen in a module using .mscorlib if System.Object is referenced
		/// before it has been defined.
		/// </summary>
		SystemObjectUndefined = 26,
		/// <summary>
		/// Happens if a base class is specified for an interface definition.
		/// </summary>
		BaseClassInInterface = 27,
		/// <summary>
		/// Happens if an embedded resource file could not be read.
		/// </summary>
		ResourceFileError = 28,
		/// <summary>
		/// Happens if the overriding method of a .override directive could not
		/// be resolved.
		/// </summary>
		InvalidOverrideMethod = 29,
	}
}
