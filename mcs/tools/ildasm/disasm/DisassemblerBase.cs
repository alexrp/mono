// 
// DisassemblerBase.cs
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
using System.IO;
using System.Text;
using Mono.Cecil;

namespace Mono.ILDasm {
	internal abstract class DisassemblerBase {
		public CodeWriter Writer { get; private set; }
		
		protected DisassemblerBase (TextWriter output)
		{
			Writer = new CodeWriter (output);
		}
		
		protected DisassemblerBase (CodeWriter writer)
		{
			Writer = writer;
		}
		
		public static bool EscapeAlways { get; set; }
		
		public static string Escape (string identifier)
		{
			if (EscapeAlways)
				return "'" + EscapeString (identifier) + "'";
			
			// Since keywords in ILAsm don't have any odd symbols, we
			// can just escape them with apostrophes.
			if (KeywordTable.Keywords.ContainsKey (identifier))
				return "'" + identifier + "'";
			
			if (!IsIdentifierStartChar (identifier [0]))
				return "'" + EscapeString (identifier) + "'";
			
			foreach (var chr in identifier)
				if (!IsIdentifierChar (chr))
					return "'" + EscapeString (identifier) + "'";
			
			return identifier;
		}
		
		public static string EscapeString (string str)
		{
			return str.Replace ("'", "\\'").Replace ("\\", "\\\\");
		}
		
		public static string ToByteList (byte[] bytes)
		{
			var sb = new StringBuilder ("( ");
			
			for (var i = 0; i < bytes.Length; i++)
			{
				sb.Append (bytes [i].ToString ("X2"));
				
				if (i != bytes.Length - 1) {
					if ((i + 1) % 20 == 0)
						sb.AppendLine ().Append ("  ");
					else
						sb.Append (" ");
				} else
					sb.Append (" ");
			}
			
			return sb.Append (")").ToString ();
		}
		
		// TODO: These methods may not quite be in line with MS.NET...
		
		public static bool IsIdentifierStartChar (char chr)
		{
			return char.IsLetter (chr) || "_$@?`".IndexOf (chr) != -1;
		}
		
		public static bool IsIdentifierChar (char chr)
		{
			return char.IsLetterOrDigit (chr) || "_$@?`.".IndexOf (chr) != -1;
		}
		
		public static string Stringize (MethodCallingConvention conv)
		{
			switch (conv)
			{
			case MethodCallingConvention.Default:
				return "default";
			case MethodCallingConvention.VarArg:
				return "vararg";
			case MethodCallingConvention.C:
				return "unmanaged cdecl";
			case MethodCallingConvention.StdCall:
				return "unmanaged stdcall";
			case MethodCallingConvention.ThisCall:
				return "unmanaged thiscall";
			case MethodCallingConvention.FastCall:
				return "unmanaged fastcall";
			default:
				return "callconv (" + conv.ToInt32Hex () + ")";
			}
		}
		
		public static string Stringize (TypeReference type)
		{
			if (type is ArrayType)
				return Stringize ((ArrayType) type);
			else if (type is ByReferenceType)
				return Stringize ((ByReferenceType) type);
			else if (type is FunctionPointerType)
				return Stringize ((FunctionPointerType) type);
			else if (type is OptionalModifierType)
				return Stringize ((OptionalModifierType) type);
			else if (type is RequiredModifierType)
				return Stringize ((RequiredModifierType) type);
			else if (type is PinnedType)
				return Stringize ((PinnedType) type);
			else if (type is PointerType)
				return Stringize ((PointerType) type);
			else if (type is SentinelType)
				return Stringize ((SentinelType) type);
			else if (type is GenericParameter)
				return Stringize ((GenericParameter) type);
			else {
				var sb = new StringBuilder ();
				
				if (type.Scope is ModuleReference)
					sb.AppendFormat ("[.module {0}] ", type.Scope.Name);
				else
					sb.AppendFormat ("[{0}] ", type.Scope.Name);
				
				sb.Append (type.FullName);
				
				return sb.ToString ();
			}
		}
		
		public static string Stringize (ArrayType type)
		{
			var sb = new StringBuilder (Stringize (type.ElementType));
			
			if (type.IsVector)
				return sb.Append ("[]").ToString ();
			
			sb.Append ("[");
			
			for (var i = 0; i < type.Rank; i++) {
				var dim = type.Dimensions [i];
				
				if (dim.LowerBound == null && dim.UpperBound == null)
					sb.Append ("...");
				else if (dim.LowerBound == null)
					sb.Append (dim.UpperBound);
				else if (dim.UpperBound == null)
					sb.AppendFormat ("{0} ...", dim.LowerBound);
				else
					sb.AppendFormat ("{0} ... {1}", dim.LowerBound, dim.UpperBound);
				
				if (i != type.Rank - 1)
					sb.Append (", ");
			}
			
			return sb.Append ("]").ToString ();
		}
		
		public static string Stringize (ByReferenceType type)
		{
			return Stringize (type.ElementType) + "&";
		}
		
		public static string Stringize (FunctionPointerType type)
		{
			var sb = new StringBuilder ("method ");
			
			if (type.HasThis)
				sb.Append ("instance ");
			
			if (type.ExplicitThis)
				sb.Append ("explicit ");
			
			sb.AppendFormat ("{0} ", Stringize (type.CallingConvention));
			
			sb.Append (Stringize (type.ReturnType));
			sb.Append (" * ");
			sb.Append ("(");
			
			for (var i = 0; i < type.Parameters.Count; i++) {
				sb.Append (Stringize (type.Parameters [i].ParameterType));
				
				if (i != type.Parameters.Count - 1)
					sb.Append (", ");
			}
			
			return sb.Append (")").ToString ();
		}
		
		public static string Stringize (OptionalModifierType type)
		{
			return Stringize (type.ElementType) +
				"modopt (" + Stringize (type.ModifierType) + ")";
		}
		
		public static string Stringize (RequiredModifierType type)
		{
			return Stringize (type.ElementType) +
				"modreq (" + Stringize (type.ModifierType) + ")";
		}
		
		public static string Stringize (PinnedType type)
		{
			return Stringize (type.ElementType) + " pinned";
		}
		
		public static string Stringize (PointerType type)
		{
			return Stringize (type.ElementType) + "*";
		}
		
		public static string Stringize (SentinelType type)
		{
			return "...";
		}
		
		public static string Stringize (GenericParameter type)
		{
			return "TODO: GenericParameter";
		}
	}
}
