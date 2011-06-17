using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Mdb;

namespace Mono.ILAsm {
	internal sealed class CodeGenerator {
		Corlib corlib;
		Report report;
		
		public Corlib Corlib {
			get {
				return corlib ?? (corlib = GetCorlib ());
			}
		}
		
		public ModuleDefinition CurrentModule { get; private set; }
		
		public string CurrentNamespace { get; set; }
		
		public TypeDefinition CurrentType { get; set; }
		
		public MethodDefinition CurrentMethod { get; set; }
		
		public AssemblyNameReference CurrentAssemblyReference { get; set; }
		
		public IGenericParameterProvider CurrentGenericParameterProvider { get; set; }
		
		public bool HasAssemblyDirective { get; set; }
		
		public bool HasModuleDirective { get; set; }
		
		public bool HasEntryPoint { get; set; }
		
		public bool DebuggingSymbols { get; set; }
		
		public bool IsCorlib { get; set; }
		
		public Dictionary<string, AliasedAssemblyNameReference> AliasedAssemblyReferences { get; private set; }
		
		public Dictionary<string, object> DataConstants { get; private set; }
		
		public Dictionary<TypeDefinition, Dictionary<FieldDefinition, string>> FieldDataMappings { get; private set; }
		
		public CodeGenerator (Report report, string moduleName, Target target)
		{
			this.report = report;
			AliasedAssemblyReferences = new Dictionary<string, AliasedAssemblyNameReference> ();
			DataConstants = new Dictionary<string, object> ();
			FieldDataMappings = new Dictionary<TypeDefinition, Dictionary<FieldDefinition, string>> ();
			CurrentModule = ModuleDefinition.CreateModule (moduleName,
				target == Target.Dll ? ModuleKind.Dll : ModuleKind.Console);
		}
		
		private void EmitDataConstants ()
		{
			// This implementation is far from standard-compliant. We can't
			// currently emit actual data constants, so we emulate the
			// behavior of field -> data constant mappings by copying the
			// values from data constants to the InitialValue of fields.
			
			foreach (var type in FieldDataMappings) {
				foreach (var mapping in type.Value) {
					var label = mapping.Value;
					var data = DataConstants.TryGet (label);
					
					if (data == null)
						report.WriteError (Error.InvalidDataConstantLabel,
							"Could not find data location '{0}'.", label);
					
					using (var bin = new BinaryWriter (new MemoryStream ())) {
						if (data is string)
							bin.Write (Encoding.Unicode.GetBytes ((string) data));
						else if (data is byte[])
							bin.Write ((byte[]) data);
						else if (data is float)
							bin.Write ((float) data);
						else if (data is double)
							bin.Write ((double) data);
						else if (data is long)
							bin.Write ((long) data);
						else if (data is int)
							bin.Write ((int) data);
						else if (data is short)
							bin.Write ((short) data);
						else if (data is byte)
							bin.Write ((byte) data);
						
						var stream = bin.BaseStream;
						var length = (int) stream.Length;
						var value = new byte [length];
						
						stream.Position = 0;
						stream.Read (value, 0, length);
						
						mapping.Key.InitialValue = value;
					}
				}
			}
		}
		
		public void Write (string outputFile)
		{
			EmitDataConstants ();
			
			CurrentModule.Write (outputFile, new WriterParameters {
				SymbolWriterProvider = new MdbWriterProvider (),
				WriteSymbols = DebuggingSymbols,
			});
		}
		
		public Dictionary<FieldDefinition, string> GetFieldDataMapping (TypeDefinition type)
		{
			var mapping = FieldDataMappings.TryGet (type);
			
			if (mapping != null)
				return mapping;
			
			mapping = new Dictionary<FieldDefinition, string> ();
			FieldDataMappings.Add (type, mapping);
			
			return mapping;
		}
		
		private AssemblyNameReference ResolveAssemblyReference (string name)
		{
			// Attempt to resolve the assembly in the GAC and insert its
			// version and public key token in a reference.
			
			report.WriteWarning (Warning.AutoResolvingAssembly,
				"Attempting to resolve assembly: {0}", name);
			
			var asmName = new AssemblyNameReference (name, null);
			var asm = CurrentModule.AssemblyResolver.Resolve (asmName);
			
			if (asm != null) {
				asmName.Version = asm.Name.Version;
				asmName.PublicKeyToken = asm.Name.PublicKeyToken;
				return asmName;
			}
			
			report.WriteWarning (Warning.AutoResolutionFailed,
				"Could not resolve assembly: {0}", name);
			
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
			// thus error if a reference hasn't been declared. If the name
			// matches the current module, and we're explicitly searching for
			// a module, we return that.
			
			if (module && name == CurrentModule.Name)
				return CurrentModule;
			
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
				report.WriteError (Error.UndeclaredModuleReference,
					"Use of undeclared module: {0}", name);
			
			// No dice. Let's try automatically resolving it as an assembly.
			var gacAsm = ResolveAssemblyReference (name);
			if (gacAsm != null) {
				CurrentModule.AssemblyReferences.Add (gacAsm);
				return gacAsm;
			}
			
			// OK, so all attempts failed. We'll just assume it works...
			return new AssemblyNameReference (name, new Version ());
		}
		
		private Corlib GetCorlib ()
		{
			const string corlibStr = "mscorlib";
			
			var asm = GetAliasedAssemblyReference (corlibStr);
			if (asm != null)
				return new Corlib (CurrentModule, asm);
			
			// TODO: Should we error if we can't resolve it?
			asm = ResolveAssemblyReference (corlibStr) ??
				new AssemblyNameReference (corlibStr, new Version ());
			CurrentModule.AssemblyReferences.Add (asm);
			
			return new Corlib (CurrentModule, asm);
		}
	}
}
