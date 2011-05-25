using System;
using Mono.Cecil;

namespace Mono.ILAsm {
	public sealed class CodeGenerator {
		bool has_entry_point;
		
		public TypeManager TypeManager { get; private set; }
		
		public string CurrentNamespace { get; set; }
		
		public AssemblyDefinition CurrentAssembly { get; set; }
		
		public ModuleDefinition CurrentModule { get; set; }
		
		public TypeDefinition CurrentType { get; set; }
		
		public MethodDefinition CurrentMethod { get; set; }
		
		public bool HasEntryPoint {
			get {
				return has_entry_point;
			}
			set {
				if (has_entry_point)
					Report.Error ("Multiple .entrypoint declarations.");
				
				has_entry_point = value;
			}
		}
		
		public CodeGenerator (string moduleName, bool dll, bool debuggingInfo)
		{
			TypeManager = new TypeManager ();
		}
		
		public bool IsThisAssembly(AssemblyNameReference name)
		{
			return name.FullName == CurrentAssembly.Name.FullName;
		}
		
		public void Write ()
		{
		}
	}
}
