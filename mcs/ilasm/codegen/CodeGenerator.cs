using System;
using Mono.Cecil;

namespace Mono.ILAsm {
	public sealed class CodeGenerator {
		public ModuleDefinition CurrentModule { get; private set; }
		
		public string CurrentNamespace { get; internal set; }
		
		public bool HasModuleDirective { get; internal set; }
		
		public bool HasAssemblyDirective { get; internal set; }
		
		public TypeDefinition CurrentType { get; internal set; }
		
		public MethodDefinition CurrentMethod { get; internal set; }
		
		public bool HasEntryPoint { get; internal set; }
		
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
