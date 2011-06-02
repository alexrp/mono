using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Mdb;

namespace Mono.ILAsm {
	public sealed class CodeGenerator {
		Corlib corlib;
		
		public ModuleDefinition CurrentModule { get; private set; }
		
		public string CurrentNamespace { get; internal set; }
		
		public bool HasModuleDirective { get; internal set; }
		
		public bool HasAssemblyDirective { get; internal set; }
		
		public TypeDefinition CurrentType { get; internal set; }
		
		public MethodDefinition CurrentMethod { get; internal set; }
		
		public AssemblyNameReference CurrentAssemblyReference { get; internal set; }
		
		public bool HasEntryPoint { get; internal set; }
		
		public bool DebuggingSymbols { get; set; }
		
		public Dictionary<string, AliasedAssemblyNameReference> AliasedAssemblyReferences { get; private set; }
		
		public CodeGenerator (string moduleName, Target target)
		{
			AliasedAssemblyReferences = new Dictionary<string, AliasedAssemblyNameReference> ();
			CurrentModule = ModuleDefinition.CreateModule (moduleName,
				target == Target.Dll ? ModuleKind.Dll : ModuleKind.Console);
		}
		
		public void Write (string outputFile)
		{
			CurrentModule.Write (outputFile, new WriterParameters {
				SymbolWriterProvider = new MdbWriterProvider (),
				WriteSymbols = DebuggingSymbols,
			});
		}
		
		private AssemblyNameReference ResolveAssemblyReference (string name)
		{
			// Attempt to resolve the assembly in the GAC and insert its
			// version and public key token in a reference.
			
			var asmName = new AssemblyNameReference (name, null);
			var asm = CurrentModule.AssemblyResolver.Resolve (asmName);
			
			if (asm != null) {
				asmName.Version = asm.Name.Version;
				asmName.PublicKeyToken = asm.Name.PublicKeyToken;
				return asmName;
			}
			
			return null;
		}
		
		public AssemblyNameReference GetAssemblyReference (string name)
		{
			foreach (var asm in CurrentModule.AssemblyReferences)
				if (asm.Name == name)
					return asm;
			
			return null;
		}
		
		public AssemblyNameReference GetAliasedAssemblyReference (string name)
		{
			return GetAssemblyReference (name) ?? AliasedAssemblyReferences.TryGet (name);
		}
		
		public ModuleReference GetModuleReference (string name)
		{
			foreach (var mod in CurrentModule.ModuleReferences)
				if (mod.Name == name)
					return mod;
			
			return null;
		}
		
		public IMetadataScope GetScope (string name, bool module)
		{
			// OK, so this behavior is a little messed up. Generally, the
			// procedure goes like this: If we have an assembly with the
			// given name, pick that. Otherwise, attempt to locate a module.
			// If that fails, we assume that the code intends to point at an
			// undeclared assembly, which we try to resolve automatically. If,
			// however, that fails, we just blindly emit an assembly name
			// reference and hope for the best... Note that if the module
			// parameter is true, we're explicitly searching for a module, and
			// thus error if a reference hasn't been declared.
			
			if (!module) {
				var asm = GetAliasedAssemblyReference (name);
				if (asm != null)
					return asm;
			}
			
			foreach (var mod in CurrentModule.ModuleReferences)
				if (mod.Name == name)
					return mod;
			
			// If we don't have a module by now, something went wrong.
			if (module)
				Report.WriteError (Error.UndeclaredModuleReference,
					"Use of undeclared module: {0}", name);
			
			Report.WriteWarning (Warning.AutoResolvingAssembly,
				"Attempting to resolve assembly: {0}", name);
			
			// No dice. Let's try automatically resolving it as an assembly.
			var gacAsm = ResolveAssemblyReference (name);
			if (gacAsm != null) {
				CurrentModule.AssemblyReferences.Add (gacAsm);
				return gacAsm;
			}
			
			Report.WriteWarning (Warning.AutoResolutionFailed,
				"Could not resolve assembly: {0}", name);
			
			// OK, so all attempts failed. We'll just assume it works...
			return new AssemblyNameReference (name, new Version ());
		}
		
		public Corlib GetCorlib ()
		{
			if (corlib != null)
				return corlib;
			
			const string corlibStr = "mscorlib";
			
			var asm = GetAliasedAssemblyReference (corlibStr);
			if (asm != null)
				return corlib = new Corlib (CurrentModule, asm);
			
			// TODO: Should we error if we can't resolve it?
			asm = ResolveAssemblyReference (corlibStr) ??
				new AssemblyNameReference (corlibStr, new Version ());
			CurrentModule.AssemblyReferences.Add (asm);
			
			return corlib = new Corlib (CurrentModule, asm);
		}
	}
}
