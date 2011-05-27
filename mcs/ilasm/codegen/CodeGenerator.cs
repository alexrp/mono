using System;
using Mono.Cecil;

namespace Mono.ILAsm {
	public sealed class CodeGenerator {
		bool has_entry_point;
		public ModuleDefinition CurrentModule { get; private set; }
		
		public string CurrentNamespace { get; internal set; }
		
		public bool HasModuleDirective { get; internal set; }
		
		public bool HasAssemblyDirective { get; internal set; }
		
		public TypeDefinition CurrentType { get; internal set; }
		
		public MethodDefinition CurrentMethod { get; internal set; }
		
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
			var kind = dll ? ModuleKind.Dll : ModuleKind.Console;
			CurrentModule = ModuleDefinition.CreateModule (moduleName, kind);
		}
		
		public void Write (string outputFile)
		{
			CurrentModule.Write (outputFile);
		}
	}
}
