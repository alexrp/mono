// 
// ModuleDisassembler.cs
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
using Mono.Cecil;

namespace Mono.ILDasm {
	internal sealed class ModuleDisassembler : DisassemblerBase {
		readonly ModuleDefinition module;
		
		public bool VerbalCustomAttributes { get; set; }
		
		public bool NoCustomAttributes { get; set; }
		
		public bool RawBytes { get; set; }
		
		public bool RawExceptionHandlers { get; set; }
		
		public bool ShowMetadataTokens { get; set; }
		
		public Visibility? Visibility { get; set; }
		
		public bool NoCil { get; set; }
		
		public ModuleDisassembler (TextWriter output, ModuleDefinition module)
			: base (output)
		{
			this.module = module;
		}
		
		public void Disassemble ()
		{
			WriteAssemblyManifest ();
			Writer.WriteLine ();
			
			WriteAssemblyReferences ();
			Writer.WriteLine ();
			
			WriteModuleReferences ();
			Writer.WriteLine ();
			
			WriteModuleManifest ();
			Writer.WriteLine ();
		}
		
		void WriteAssemblyManifest ()
		{
			var asm = module.Assembly.Name;
			
			Writer.Write (".assembly ");
			
			if (asm.IsRetargetable)
				Writer.Write ("retargetable ");
			
			Writer.WriteLine (Escape (asm.Name));
			Writer.OpenBracket ();
			
			if (asm.HasPublicKey) {
				Writer.WriteIndentedLine (".publickey =");
				Writer.Indent ();
				
				Writer.WriteIndentedLine (ToByteList (asm.PublicKey));
				
				Writer.Dedent ();
			}
			
			var ver = asm.Version;
			if (ver != new Version (0, 0, 0, 0))
				Writer.WriteIndentedLine (".ver {0}:{1}:{2}:{3}", ver.Major,
					ver.Minor, ver.Build, ver.Revision);
			
			// Use .locale for MS.NET compatibility.
			if (asm.Culture != string.Empty)
				Writer.WriteIndentedLine (".locale '{0}'",
					EscapeString (asm.Culture));
			
			if (asm.HashAlgorithm != AssemblyHashAlgorithm.None)
				Writer.WriteIndentedLine (".hash algorithm {0}",
					asm.HashAlgorithm.ToInt32Hex ());
			
			Writer.CloseBracket ();
		}
		
		void WriteAssemblyReferences ()
		{
			foreach (var asm in module.AssemblyReferences) {
				Writer.Write (".assembly extern ");
				
				if (asm.IsRetargetable)
					Writer.Write ("retargetable ");
				
				Writer.WriteLine (Escape (asm.Name));
				Writer.OpenBracket ();
				
				if (asm.HasPublicKey) {
					Writer.WriteIndentedLine (".publickey =");
					Writer.Indent ();
					
					Writer.WriteIndentedLine (ToByteList (asm.PublicKey));
					
					Writer.Dedent ();
				} else if (asm.PublicKeyToken != null && asm.PublicKeyToken.Length != 0) {
					Writer.WriteIndentedLine (".publickeytoken =");
					Writer.Indent ();
					
					Writer.WriteIndentedLine (ToByteList (asm.PublicKeyToken));
					
					Writer.Dedent ();
				}
				
				var ver = asm.Version;
				if (ver != new Version (0, 0, 0, 0))
					Writer.WriteIndentedLine (".ver {0}:{1}:{2}:{3}", ver.Major,
						ver.Minor, ver.Build, ver.Revision);
				
				// Use .locale for MS.NET compatibility.
				if (asm.Culture != string.Empty)
					Writer.WriteIndentedLine (".locale '{0}'",
						EscapeString (asm.Culture));
				
				if (asm.HashAlgorithm != AssemblyHashAlgorithm.None)
					Writer.WriteIndentedLine (".hash algorithm {0}",
						asm.HashAlgorithm.ToInt32Hex ());
				
				Writer.CloseBracket ();
			}
		}
		
		void WriteModuleReferences ()
		{
			foreach (var mod in module.ModuleReferences)
				Writer.WriteLine (".module extern {0}", Escape (mod.Name));
		}
		
		void WriteModuleManifest ()
		{
		}
	}
}
