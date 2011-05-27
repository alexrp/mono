using System;
using Mono.Cecil;

namespace Mono.ILAsm {
	public sealed class CodeGenerator {
		bool has_entry_point;
		
		public string CurrentNamespace { get; set; }
		
		public ModuleDefinition CurrentModule { get; set; }
		
		public bool HasModuleDirective { get; set; }
		
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
			var kind = dll ? ModuleKind.Dll : ModuleKind.Console;
			CurrentModule = ModuleDefinition.CreateModule (moduleName, kind);
		}
		
		public void Write (string outputFile)
		{
			CurrentModule.Write (outputFile);
		}
	}
}
