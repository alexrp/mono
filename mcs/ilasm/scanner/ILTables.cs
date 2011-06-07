// ILTables.cs
//
// (C) Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Cecil.Cil;

namespace Mono.ILAsm {
	public static class ILTables {
		public sealed class ReadOnlyDictionaryAdapter<TKey, TValue> : IDictionary<TKey, TValue> {
			readonly IDictionary<TKey, TValue> dict;
			
			public ReadOnlyDictionaryAdapter (IDictionary<TKey, TValue> dict)
			{
				this.dict = dict;
			}
			
			void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
			{
				throw new NotSupportedException ();
			}

			public bool ContainsKey (TKey key)
			{
				return dict.ContainsKey (key);
			}

			bool IDictionary<TKey, TValue>.Remove (TKey key)
			{
				throw new NotSupportedException ();
			}

			public bool TryGetValue (TKey key, out TValue value)
			{
				return dict.TryGetValue (key, out value);
			}

			public TValue this [TKey key] {
				get {
					return dict [key];
				}
				set {
					throw new NotSupportedException ();
				}
			}

			public ICollection<TKey> Keys {
				get {
					return dict.Keys;
				}
			}

			public ICollection<TValue> Values {
				get {
					return dict.Values;
				}
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return ((IEnumerable) dict).GetEnumerator ();
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
			{
				return dict.GetEnumerator ();
			}

			void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
			{
				throw new NotSupportedException ();
			}

			void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
			{
				throw new NotSupportedException ();
			}

			public bool Contains (KeyValuePair<TKey, TValue> item)
			{
				return dict.Contains (item);
			}

			public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				dict.CopyTo (array, arrayIndex);
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
			{
				throw new NotSupportedException ();
			}

			public int Count {
				get {
					return dict.Count;
				}
			}

			public bool IsReadOnly {
				get {
					return true;
				}
			}
		}
		
		static ILTables ()
		{
			var directives = new Dictionary<string, ILToken> (300);
			
			directives [".addon"] = new ILToken (Token.D_ADDON, ".addon");
			directives [".assembly"] = new ILToken (Token.D_ASSEMBLY, ".assembly");
			directives [".backing"] = new ILToken (Token.D_BACKING, ".backing");
			directives [".blob"] = new ILToken (Token.D_BLOB, ".blob");
			directives [".capability"] = new ILToken (Token.D_CAPABILITY, ".capability");
			directives [".cctor"] = new ILToken (Token.D_CCTOR, ".cctor");
			directives [".class"] = new ILToken (Token.D_CLASS, ".class");
			directives [".comtype"] = new ILToken (Token.D_COMTYPE, ".comtype");
			directives [".config"] = new ILToken (Token.D_CONFIG, ".config");
			directives [".imagebase"] = new ILToken (Token.D_IMAGEBASE, ".imagebase");
			directives [".corflags"] = new ILToken (Token.D_CORFLAGS, ".corflags");
			directives [".ctor"] = new ILToken (Token.D_CTOR, ".ctor");
			directives [".custom"] = new ILToken (Token.D_CUSTOM, ".custom");
			directives [".data"] = new ILToken (Token.D_DATA, ".data");
			directives [".emitbyte"] = new ILToken (Token.D_EMITBYTE, ".emitbyte");
			directives [".entrypoint"] = new ILToken (Token.D_ENTRYPOINT, ".entrypoint");
			directives [".event"] = new ILToken (Token.D_EVENT, ".event");
			directives [".exeloc"] = new ILToken (Token.D_EXELOC, ".exeloc");
			directives [".export"] = new ILToken (Token.D_EXPORT, ".export");
			directives [".field"] = new ILToken (Token.D_FIELD, ".field");
			directives [".file"] = new ILToken (Token.D_FILE, ".file");
			directives [".fire"] = new ILToken (Token.D_FIRE, ".fire");
			directives [".get"] = new ILToken (Token.D_GET, ".get");
			directives [".hash"] = new ILToken (Token.D_HASH, ".hash");
			directives [".implicitcom"] = new ILToken (Token.D_IMPLICITCOM, ".implicitcom");
			directives [".language"] = new ILToken (Token.D_LANGUAGE, ".language");
			directives [".line"] = new ILToken (Token.D_LINE, ".line");
			directives ["#line"] = new ILToken (Token.D_XLINE, "#line");
			directives [".locale"] = new ILToken (Token.D_LOCALE, ".locale");
			directives [".locals"] = new ILToken (Token.D_LOCALS, ".locals");
			directives [".manifestres"] = new ILToken (Token.D_MANIFESTRES, ".manifestres");
			directives [".maxstack"] = new ILToken (Token.D_MAXSTACK, ".maxstack");
			directives [".method"] = new ILToken (Token.D_METHOD, ".method");
			directives [".mime"] = new ILToken (Token.D_MIME, ".mime");
			directives [".module"] = new ILToken (Token.D_MODULE, ".module");
			directives [".mresource"] = new ILToken (Token.D_MRESOURCE, ".mresource");
			directives [".namespace"] = new ILToken (Token.D_NAMESPACE, ".namespace");
			directives [".originator"] = new ILToken (Token.D_ORIGINATOR, ".originator");
			directives [".os"] = new ILToken (Token.D_OS, ".os");
			directives [".other"] = new ILToken (Token.D_OTHER, ".other");
			directives [".override"] = new ILToken (Token.D_OVERRIDE, ".override");
			directives [".pack"] = new ILToken (Token.D_PACK, ".pack");
			directives [".param"] = new ILToken (Token.D_PARAM, ".param");
			directives [".permission"] = new ILToken (Token.D_PERMISSION, ".permission");
			directives [".permissionset"] = new ILToken (Token.D_PERMISSIONSET, ".permissionset");
			directives [".processor"] = new ILToken (Token.D_PROCESSOR, ".processor");
			directives [".property"] = new ILToken (Token.D_PROPERTY, ".property");
			directives [".publickey"] = new ILToken (Token.D_PUBLICKEY, ".publickey");
			directives [".publickeytoken"] = new ILToken (Token.D_PUBLICKEYTOKEN, ".publickeytoken");
			directives [".removeon"] = new ILToken (Token.D_REMOVEON, ".removeon");
			directives [".set"] = new ILToken (Token.D_SET, ".set");
			directives [".size"] = new ILToken (Token.D_SIZE, ".size");
			directives [".stackreserve"] = new ILToken (Token.D_STACKRESERVE, ".stackreserve");
			directives [".subsystem"] = new ILToken (Token.D_SUBSYSTEM, ".subsystem");
			directives [".title"] = new ILToken (Token.D_TITLE, ".title");
			directives [".try"] = new ILToken (Token.D_TRY, ".try");
			directives [".ver"] = new ILToken (Token.D_VER, ".ver");
			directives [".vtable"] = new ILToken (Token.D_VTABLE, ".vtable");
			directives [".vtentry"] = new ILToken (Token.D_VTENTRY, ".vtentry");
			directives [".vtfixup"] = new ILToken (Token.D_VTFIXUP, ".vtfixup");
			directives [".zeroinit"] = new ILToken (Token.D_ZEROINIT, ".zeroinit");
			directives [".this"] = new ILToken (Token.D_THIS, ".this");
			directives [".base"] = new ILToken (Token.D_BASE, ".base");
			directives [".nester"] = new ILToken (Token.D_NESTER, ".nester");
			directives [".typelist"] = new ILToken (Token.D_TYPELIST, ".typelist");
			directives [".mscorlib"] = new ILToken (Token.D_MSCORLIB, ".mscorlib");
			
			Directives = new ReadOnlyDictionaryAdapter<string, ILToken> (directives);
			
			var keywords = new Dictionary<string, ILToken> (300);
			
			keywords ["at"] = new ILToken (Token.K_AT, "at");
			keywords ["as"] = new ILToken (Token.K_AS, "as");
			keywords ["implicitcom"] = new ILToken (Token.K_IMPLICITCOM, "implicitcom");
			keywords ["implicitres"] = new ILToken (Token.K_IMPLICITRES, "implicitres");
			keywords ["extern"] = new ILToken (Token.K_EXTERN, "extern");
			keywords ["instance"] = new ILToken (Token.K_INSTANCE, "instance");
			keywords ["explicit"] = new ILToken (Token.K_EXPLICIT, "explicit");
			keywords ["default"] = new ILToken (Token.K_DEFAULT, "default");
			keywords ["vararg"] = new ILToken (Token.K_VARARG, "vararg");
			keywords ["unmanaged"] = new ILToken (Token.K_UNMANAGED, "unmanaged");
			keywords ["cdecl"] = new ILToken (Token.K_CDECL, "cdecl");
			keywords ["stdcall"] = new ILToken (Token.K_STDCALL, "stdcall");
			keywords ["thiscall"] = new ILToken (Token.K_THISCALL, "thiscall");
			keywords ["fastcall"] = new ILToken (Token.K_FASTCALL, "fastcall");
			keywords ["marshal"] = new ILToken (Token.K_MARSHAL, "marshal");
			keywords ["in"] = new ILToken (Token.K_IN, "in");
			keywords ["out"] = new ILToken (Token.K_OUT, "out");
			keywords ["opt"] = new ILToken (Token.K_OPT, "opt");
			keywords ["static"] = new ILToken (Token.K_STATIC, "static");
			keywords ["public"] = new ILToken (Token.K_PUBLIC, "public");
			keywords ["private"] = new ILToken (Token.K_PRIVATE, "private");
			keywords ["family"] = new ILToken (Token.K_FAMILY, "family");
			keywords ["initonly"] = new ILToken (Token.K_INITONLY, "initonly");
			keywords ["rtspecialname"] = new ILToken (Token.K_RTSPECIALNAME, "rtspecialname");
			keywords ["specialname"] = new ILToken (Token.K_SPECIALNAME, "specialname");
			keywords ["assembly"] = new ILToken (Token.K_ASSEMBLY, "assembly");
			keywords ["famandassem"] = new ILToken (Token.K_FAMANDASSEM, "famandassem");
			keywords ["famorassem"] = new ILToken (Token.K_FAMORASSEM, "famorassem");
			keywords ["privatescope"] = new ILToken (Token.K_PRIVATESCOPE, "privatescope");
			keywords ["literal"] = new ILToken (Token.K_LITERAL, "literal");
			keywords ["notserialized"] = new ILToken (Token.K_NOTSERIALIZED, "notserialized");
			keywords ["value"] = new ILToken (Token.K_VALUE, "value");
			keywords ["not_in_gc_heap"] = new ILToken (Token.K_NOT_IN_GC_HEAP, "not_in_gc_heap");
			keywords ["interface"] = new ILToken (Token.K_INTERFACE, "interface");
			keywords ["sealed"] = new ILToken (Token.K_SEALED, "sealed");
			keywords ["abstract"] = new ILToken (Token.K_ABSTRACT, "abstract");
			keywords ["auto"] = new ILToken (Token.K_AUTO, "auto");
			keywords ["sequential"] = new ILToken (Token.K_SEQUENTIAL, "sequential");
			keywords ["ansi"] = new ILToken (Token.K_ANSI, "ansi");
			keywords ["unicode"] = new ILToken (Token.K_UNICODE, "unicode");
			keywords ["autochar"] = new ILToken (Token.K_AUTOCHAR, "autochar");
			keywords ["bestfit"] = new ILToken (Token.K_BESTFIT, "bestfit");
			keywords ["charmaperror"] = new ILToken (Token.K_CHARMAPERROR, "charmaperror");
			keywords ["import"] = new ILToken (Token.K_IMPORT, "import");
			keywords ["serializable"] = new ILToken (Token.K_SERIALIZABLE, "serializable");
			keywords ["nested"] = new ILToken (Token.K_NESTED, "nested");
			keywords ["lateinit"] = new ILToken (Token.K_LATEINIT, "lateinit");
			keywords ["extends"] = new ILToken (Token.K_EXTENDS, "extends");
			keywords ["implements"] = new ILToken (Token.K_IMPLEMENTS, "implements");
			keywords ["final"] = new ILToken (Token.K_FINAL, "final");
			keywords ["virtual"] = new ILToken (Token.K_VIRTUAL, "virtual");
			keywords ["hidebysig"] = new ILToken (Token.K_HIDEBYSIG, "hidebysig");
			keywords ["newslot"] = new ILToken (Token.K_NEWSLOT, "newslot");
			keywords ["unmanagedexp"] = new ILToken (Token.K_UNMANAGEDEXP, "unmanagedexp");
			keywords ["pinvokeimpl"] = new ILToken (Token.K_PINVOKEIMPL, "pinvokeimpl");
			keywords ["nomangle"] = new ILToken (Token.K_NOMANGLE, "nomangle");
			keywords ["ole"] = new ILToken (Token.K_OLE, "ole");
			keywords ["lasterr"] = new ILToken (Token.K_LASTERR, "lasterr");
			keywords ["winapi"] = new ILToken (Token.K_WINAPI, "winapi");
			keywords ["platformapi"] = new ILToken (Token.K_PLATFORMAPI, "platformapi");
			keywords ["native"] = new ILToken (Token.K_NATIVE, "native");
			keywords ["il"] = new ILToken (Token.K_IL, "il");
			keywords ["cil"] = new ILToken (Token.K_CIL, "cil");
			keywords ["optil"] = new ILToken (Token.K_OPTIL, "optil");
			keywords ["managed"] = new ILToken (Token.K_MANAGED, "managed");
			keywords ["forwardref"] = new ILToken (Token.K_FORWARDREF, "forwardref");
			keywords ["runtime"] = new ILToken (Token.K_RUNTIME, "runtime");
			keywords ["internalcall"] = new ILToken (Token.K_INTERNALCALL, "internalcall");
			keywords ["synchronized"] = new ILToken (Token.K_SYNCHRONIZED, "synchronized");
			keywords ["noinlining"] = new ILToken (Token.K_NOINLINING, "noinlining");
			keywords ["custom"] = new ILToken (Token.K_CUSTOM, "custom");
			keywords ["fixed"] = new ILToken (Token.K_FIXED, "fixed");
			keywords ["sysstring"] = new ILToken (Token.K_SYSSTRING, "sysstring");
			keywords ["array"] = new ILToken (Token.K_ARRAY, "array");
			keywords ["variant"] = new ILToken (Token.K_VARIANT, "variant");
			keywords ["currency"] = new ILToken (Token.K_CURRENCY, "currency");
			keywords ["syschar"] = new ILToken (Token.K_SYSCHAR, "syschar");
			keywords ["void"] = new ILToken (Token.K_VOID, "void");
			keywords ["bool"] = new ILToken (Token.K_BOOL, "bool");
			keywords ["int8"] = new ILToken (Token.K_INT8, "int8");
			keywords ["int16"] = new ILToken (Token.K_INT16, "int16");
			keywords ["int32"] = new ILToken (Token.K_INT32, "int32");
			keywords ["int64"] = new ILToken (Token.K_INT64, "int64");
			keywords ["float"] = new ILToken (Token.K_FLOAT, "float");
			keywords ["float32"] = new ILToken (Token.K_FLOAT32, "float32");
			keywords ["float64"] = new ILToken (Token.K_FLOAT64, "float64");
			keywords ["error"] = new ILToken (Token.K_ERROR, "error");
			keywords ["unsigned"] = new ILToken (Token.K_UNSIGNED, "unsigned");
			keywords ["uint"] = new ILToken (Token.K_UINT, "uint");
			keywords ["uint8"] = new ILToken (Token.K_UINT8, "uint8");
			keywords ["uint16"] = new ILToken (Token.K_UINT16, "uint16");
			keywords ["uint32"] = new ILToken (Token.K_UINT32, "uint32");
			keywords ["uint64"] = new ILToken (Token.K_UINT64, "uint64");
			keywords ["decimal"] = new ILToken (Token.K_DECIMAL, "decimal");
			keywords ["date"] = new ILToken (Token.K_DATE, "date");
			keywords ["bstr"] = new ILToken (Token.K_BSTR, "bstr");
			keywords ["lpstr"] = new ILToken (Token.K_LPSTR, "lpstr");
			keywords ["lpwstr"] = new ILToken (Token.K_LPWSTR, "lpwstr");
			keywords ["lptstr"] = new ILToken (Token.K_LPTSTR, "lptstr");
			keywords ["objectref"] = new ILToken (Token.K_OBJECTREF, "objectref");
			keywords ["iunknown"] = new ILToken (Token.K_IUNKNOWN, "iunknown");
			keywords ["idispatch"] = new ILToken (Token.K_IDISPATCH, "idispatch");
			keywords ["struct"] = new ILToken (Token.K_STRUCT, "struct");
			keywords ["safearray"] = new ILToken (Token.K_SAFEARRAY, "safearray");
			keywords ["int"] = new ILToken (Token.K_INT, "int");
			keywords ["byvalstr"] = new ILToken (Token.K_BYVALSTR, "byvalstr");
			keywords ["tbstr"] = new ILToken (Token.K_TBSTR, "tbstr");
			keywords ["lpvoid"] = new ILToken (Token.K_LPVOID, "lpvoid");
			keywords ["any"] = new ILToken (Token.K_ANY, "any");
			keywords ["float"] = new ILToken (Token.K_FLOAT, "float");
			keywords ["lpstruct"] = new ILToken (Token.K_LPSTRUCT, "lpstruct");
			keywords ["null"] = new ILToken (Token.K_NULL, "null");
			keywords ["vector"] = new ILToken (Token.K_VECTOR, "vector");
			keywords ["hresult"] = new ILToken (Token.K_HRESULT, "hresult");
			keywords ["carray"] = new ILToken (Token.K_CARRAY, "carray");
			keywords ["userdefined"] = new ILToken (Token.K_USERDEFINED, "userdefined");
			keywords ["record"] = new ILToken (Token.K_RECORD, "record");
			keywords ["filetime"] = new ILToken (Token.K_FILETIME, "filetime");
			keywords ["blob"] = new ILToken (Token.K_BLOB, "blob");
			keywords ["stream"] = new ILToken (Token.K_STREAM, "stream");
			keywords ["storage"] = new ILToken (Token.K_STORAGE, "storage");
			keywords ["streamed_object"] = new ILToken (Token.K_STREAMED_OBJECT, "streamed_object");
			keywords ["stored_object"] = new ILToken (Token.K_STORED_OBJECT, "stored_object");
			keywords ["blob_object"] = new ILToken (Token.K_BLOB_OBJECT, "blob_object");
			keywords ["cf"] = new ILToken (Token.K_CF, "cf");
			keywords ["clsid"] = new ILToken (Token.K_CLSID, "clsid");
			keywords ["method"] = new ILToken (Token.K_METHOD, "method");
			keywords ["class"] = new ILToken (Token.K_CLASS, "class");
			keywords ["pinned"] = new ILToken (Token.K_PINNED, "pinned");
			keywords ["modreq"] = new ILToken (Token.K_MODREQ, "modreq");
			keywords ["modopt"] = new ILToken (Token.K_MODOPT, "modopt");
			keywords ["typedref"] = new ILToken (Token.K_TYPEDREF, "typedref");
			keywords ["property"] = new ILToken (Token.K_PROPERTY, "property");
			keywords ["type"] = new ILToken (Token.K_TYPE, "type");
			keywords ["refany"] = new ILToken (Token.K_TYPEDREF, "typedref");
			keywords ["char"] = new ILToken (Token.K_CHAR, "char");
			keywords ["fromunmanaged"] = new ILToken (Token.K_FROMUNMANAGED, "fromunmanaged");
			keywords ["callmostderived"] = new ILToken (Token.K_CALLMOSTDERIVED, "callmostderived");
			keywords ["bytearray"] = new ILToken (Token.K_BYTEARRAY, "bytearray");
			keywords ["with"] = new ILToken (Token.K_WITH, "with");
			keywords ["init"] = new ILToken (Token.K_INIT, "init");
			keywords ["to"] = new ILToken (Token.K_TO, "to");
			keywords ["catch"] = new ILToken (Token.K_CATCH, "catch");
			keywords ["filter"] = new ILToken (Token.K_FILTER, "filter");
			keywords ["finally"] = new ILToken (Token.K_FINALLY, "finally");
			keywords ["fault"] = new ILToken (Token.K_FAULT, "fault");
			keywords ["handler"] = new ILToken (Token.K_HANDLER, "handler");
			keywords ["tls"] = new ILToken (Token.K_TLS, "tls");
			keywords ["field"] = new ILToken (Token.K_FIELD, "field");
			keywords ["request"] = new ILToken (Token.K_REQUEST, "request");
			keywords ["demand"] = new ILToken (Token.K_DEMAND, "demand");
			keywords ["assert"] = new ILToken (Token.K_ASSERT, "assert");
			keywords ["deny"] = new ILToken (Token.K_DENY, "deny");
			keywords ["permitonly"] = new ILToken (Token.K_PERMITONLY, "permitonly");
			keywords ["linkcheck"] = new ILToken (Token.K_LINKCHECK, "linkcheck");
			keywords ["inheritcheck"] = new ILToken (Token.K_INHERITCHECK, "inheritcheck");
			keywords ["reqmin"] = new ILToken (Token.K_REQMIN, "reqmin");
			keywords ["reqopt"] = new ILToken (Token.K_REQOPT, "reqopt");
			keywords ["reqrefuse"] = new ILToken (Token.K_REQREFUSE, "reqrefuse");
			keywords ["prejitgrant"] = new ILToken (Token.K_PREJITGRANT, "prejitgrant");
			keywords ["prejitdeny"] = new ILToken (Token.K_PREJITDENY, "prejitdeny");
			keywords ["noncasdemand"] = new ILToken (Token.K_NONCASDEMAND, "noncasdemand");
			keywords ["noncaslinkdemand"] = new ILToken (Token.K_NONCASLINKDEMAND, "noncaslinkdemand");
			keywords ["noncasinheritance"] = new ILToken (Token.K_NONCASINHERITANCE, "noncasinheritance");
			keywords ["readonly"] = new ILToken (Token.K_READONLY, "readonly");
			keywords ["nometadata"] = new ILToken (Token.K_NOMETADATA, "nometadata");
			keywords ["algorithm"] = new ILToken (Token.K_ALGORITHM, "algorithm");
			keywords ["fullorigin"] = new ILToken (Token.K_FULLORIGIN, "fullorigin");
			keywords ["enablejittracking"] = new ILToken (Token.K_ENABLEJITTRACKING, "enablejittracking");
			keywords ["disablejitoptimizer"] = new ILToken (Token.K_DISABLEJITOPTIMIZER, "disablejitoptimizer");
			keywords ["retargetable"] = new ILToken (Token.K_RETARGETABLE, "retargetable");
			keywords ["legacy"] = new ILToken (Token.K_LEGACY, "legacy");
			keywords ["library"] = new ILToken (Token.K_LIBRARY, "library");
			keywords ["x86"] = new ILToken (Token.K_X86, "x86");
			keywords ["amd64"] = new ILToken (Token.K_AMD64, "amd64");
			keywords ["ia64"] = new ILToken (Token.K_IA64, "ia64");
			keywords ["preservesig"] = new ILToken (Token.K_PRESERVESIG, "preservesig");
			keywords ["beforefieldinit"] = new ILToken (Token.K_BEFOREFIELDINIT, "beforefieldinit");
			keywords ["alignment"] = new ILToken (Token.K_ALIGNMENT, "alignment");
			keywords ["nullref"] = new ILToken (Token.K_NULLREF, "nullref");
			keywords ["valuetype"] = new ILToken (Token.K_VALUETYPE, "valuetype");
			keywords ["compilercontrolled"] = new ILToken (Token.K_COMPILERCONTROLLED, "compilercontrolled");
			keywords ["reqsecobj"] = new ILToken (Token.K_REQSECOBJ, "reqsecobj");
			keywords ["enum"] = new ILToken (Token.K_ENUM, "enum");
			keywords ["object"] = new ILToken (Token.K_OBJECT, "object");
			keywords ["string"] = new ILToken (Token.K_STRING, "string");
			keywords ["true"] = new ILToken (Token.K_TRUE, "true");
			keywords ["false"] = new ILToken (Token.K_FALSE, "false");
			keywords ["is"] = new ILToken (Token.K_IS, "is");
			keywords ["on"] = new ILToken (Token.K_ON, "on");
			keywords ["off"] = new ILToken (Token.K_OFF, "off");
			keywords ["strict"] = new ILToken (Token.K_STRICT, "strict");
			keywords ["mdtoken"] = new ILToken (Token.K_MDTOKEN, "mdtoken");
			
			Keywords = new ReadOnlyDictionaryAdapter<string, ILToken> (keywords);
			
			var opCodes = new Dictionary<string, ILToken> (300);
			
			opCodes ["nop"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Nop);
			opCodes ["break"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Break);
			opCodes ["ldarg.0"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldarg_0);
			opCodes ["ldarg.1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldarg_1);
			opCodes ["ldarg.2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldarg_2);
			opCodes ["ldarg.3"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldarg_3);
			opCodes ["ldloc.0"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldloc_0);
			opCodes ["ldloc.1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldloc_1);
			opCodes ["ldloc.2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldloc_2);
			opCodes ["ldloc.3"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldloc_3);
			opCodes ["stloc.0"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stloc_0);
			opCodes ["stloc.1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stloc_1);
			opCodes ["stloc.2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stloc_2);
			opCodes ["stloc.3"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stloc_3);
			opCodes ["ldnull"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldnull);
			opCodes ["ldc.i4.m1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_M1);
			opCodes ["ldc.i4.M1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_M1);
			opCodes ["ldc.i4.0"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_0);
			opCodes ["ldc.i4.1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_1);
			opCodes ["ldc.i4.2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_2);
			opCodes ["ldc.i4.3"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_3);
			opCodes ["ldc.i4.4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_4);
			opCodes ["ldc.i4.5"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_5);
			opCodes ["ldc.i4.6"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_6);
			opCodes ["ldc.i4.7"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_7);
			opCodes ["ldc.i4.8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldc_I4_8);
			opCodes ["dup"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Dup);
			opCodes ["pop"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Pop);
			opCodes ["ret"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ret);
			opCodes ["ldind.i1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_I1);
			opCodes ["ldind.u1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_U1);
			opCodes ["ldind.i2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_I2);
			opCodes ["ldind.u2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_U2);
			opCodes ["ldind.i4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_I4);
			opCodes ["ldind.u4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_U4);
			opCodes ["ldind.i8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_I8);
			opCodes ["ldind.u8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_I8);
			opCodes ["ldind.i"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_I);
			opCodes ["ldind.r4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_R4);
			opCodes ["ldind.r8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_R8);
			opCodes ["ldind.ref"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldind_Ref);
			opCodes ["stind.ref"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_Ref);
			opCodes ["stind.i1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_I1);
			opCodes ["stind.i2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_I2);
			opCodes ["stind.i4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_I4);
			opCodes ["stind.i8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_I8);
			opCodes ["stind.r4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_R4);
			opCodes ["stind.r8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_R8);
			opCodes ["add"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Add);
			opCodes ["sub"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Sub);
			opCodes ["mul"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Mul);
			opCodes ["div"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Div);
			opCodes ["div.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Div_Un);
			opCodes ["rem"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Rem);
			opCodes ["rem.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Rem_Un);
			opCodes ["and"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.And);
			opCodes ["or"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Or);
			opCodes ["xor"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Xor);
			opCodes ["shl"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Shl);
			opCodes ["shr"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Shr);
			opCodes ["shr.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Shr_Un);
			opCodes ["neg"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Neg);
			opCodes ["not"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Not);
			opCodes ["conv.i1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_I1);
			opCodes ["conv.i2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_I2);
			opCodes ["conv.i4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_I4);
			opCodes ["conv.i8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_I8);
			opCodes ["conv.r4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_R4);
			opCodes ["conv.r8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_R8);
			opCodes ["conv.u4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_U4);
			opCodes ["conv.u8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_U8);
			opCodes ["conv.r.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_R_Un);
			opCodes ["throw"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Throw);
			opCodes ["conv.ovf.i1.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I1_Un);
			opCodes ["conv.ovf.i2.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I2_Un);
			opCodes ["conv.ovf.i4.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I4_Un);
			opCodes ["conv.ovf.i8.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I8_Un);
			opCodes ["conv.ovf.u1.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U1_Un);
			opCodes ["conv.ovf.u2.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U2_Un);
			opCodes ["conv.ovf.u4.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U4_Un);
			opCodes ["conv.ovf.u8.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U8_Un);
			opCodes ["conv.ovf.i.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I_Un);
			opCodes ["conv.ovf.u.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U_Un);
			opCodes ["ldlen"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldlen);
			opCodes ["ldelem.i1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_I1);
			opCodes ["ldelem.u1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_U1);
			opCodes ["ldelem.i2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_I2);
			opCodes ["ldelem.u2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_U2);
			opCodes ["ldelem.i4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_I4);
			opCodes ["ldelem.u4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_U4);
			opCodes ["ldelem.i8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_I8);
			opCodes ["ldelem.u8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_I8);
			opCodes ["ldelem.i"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_I);
			opCodes ["ldelem.r4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_R4);
			opCodes ["ldelem.r8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_R8);
			opCodes ["ldelem.ref"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ldelem_Ref);
			opCodes ["stelem.i"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_I);
			opCodes ["stelem.i1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_I1);
			opCodes ["stelem.i2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_I2);
			opCodes ["stelem.i4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_I4);
			opCodes ["stelem.i8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_I8);
			opCodes ["stelem.r4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_R4);
			opCodes ["stelem.r8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_R8);
			opCodes ["stelem.ref"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stelem_Ref);
			opCodes ["conv.ovf.i1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I1);
			opCodes ["conv.ovf.u1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U1);
			opCodes ["conv.ovf.i2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I2);
			opCodes ["conv.ovf.u2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U2);
			opCodes ["conv.ovf.i4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I4);
			opCodes ["conv.ovf.u4"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U4);
			opCodes ["conv.ovf.i8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I8);
			opCodes ["conv.ovf.u8"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U8);
			opCodes ["conv.ovf.u1.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U1_Un);
			opCodes ["conv.ovf.u2.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U2_Un);
			opCodes ["conv.ovf.u4.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U4_Un);
			opCodes ["conv.ovf.u8.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U8_Un);
			opCodes ["conv.ovf.i1.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I1_Un);
			opCodes ["conv.ovf.i2.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I2_Un);
			opCodes ["conv.ovf.i4.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I4_Un);
			opCodes ["conv.ovf.i8.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I8_Un);
			opCodes ["ckfinite"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ckfinite);
			opCodes ["conv.u2"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_U2);
			opCodes ["conv.u1"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_U1);
			opCodes ["conv.i"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_I);
			opCodes ["conv.ovf.i"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_I);
			opCodes ["conv.ovf.u"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_Ovf_U);
			opCodes ["add.ovf"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Add_Ovf);
			opCodes ["add.ovf.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Add_Ovf_Un);
			opCodes ["mul.ovf"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Mul_Ovf);
			opCodes ["mul.ovf.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Mul_Ovf_Un);
			opCodes ["sub.ovf"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Sub_Ovf);
			opCodes ["sub.ovf.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Sub_Ovf_Un);
			opCodes ["endfinally"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Endfinally);
			opCodes ["endfault"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Endfinally);
			opCodes ["stind.i"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Stind_I);
			opCodes ["conv.u"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Conv_U);
			opCodes ["arglist"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Arglist);
			opCodes ["ceq"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Ceq);
			opCodes ["cgt"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Cgt);
			opCodes ["cgt.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Cgt_Un);
			opCodes ["clt"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Clt);
			opCodes ["clt.un"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Clt_Un);
			opCodes ["localloc"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Localloc);
			opCodes ["endfilter"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Endfilter);
			opCodes ["volatile."] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Volatile);
			opCodes ["tail."] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Tail);
			opCodes ["cpblk"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Cpblk);
			opCodes ["initblk"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Initblk);
			opCodes ["rethrow"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Rethrow);
			opCodes ["refanytype"] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Refanytype);
			opCodes ["readonly."] = new ILToken (Token.INSTR_NONE, Cecil.Cil.OpCodes.Readonly);
			opCodes ["ldarg"] = new ILToken (Token.INSTR_PARAM, Cecil.Cil.OpCodes.Ldarg);
			opCodes ["ldarga"] = new ILToken (Token.INSTR_PARAM, Cecil.Cil.OpCodes.Ldarga);
			opCodes ["starg"] = new ILToken (Token.INSTR_PARAM, Cecil.Cil.OpCodes.Starg);
			opCodes ["ldarg.s"] = new ILToken (Token.INSTR_PARAM, Cecil.Cil.OpCodes.Ldarg_S);
			opCodes ["ldarga.s"] = new ILToken (Token.INSTR_PARAM, Cecil.Cil.OpCodes.Ldarga_S);
			opCodes ["starg.s"] = new ILToken (Token.INSTR_PARAM, Cecil.Cil.OpCodes.Starg_S);
			opCodes ["ldloc"] = new ILToken (Token.INSTR_LOCAL, Cecil.Cil.OpCodes.Ldloc);
			opCodes ["ldloca"] = new ILToken (Token.INSTR_LOCAL, Cecil.Cil.OpCodes.Ldloca);
			opCodes ["stloc"] = new ILToken (Token.INSTR_LOCAL, Cecil.Cil.OpCodes.Stloc);
			opCodes ["ldloc.s"] = new ILToken (Token.INSTR_LOCAL, Cecil.Cil.OpCodes.Ldloc_S);
			opCodes ["ldloca.s"] = new ILToken (Token.INSTR_LOCAL, Cecil.Cil.OpCodes.Ldloca_S);
			opCodes ["stloc.s"] = new ILToken (Token.INSTR_LOCAL, Cecil.Cil.OpCodes.Stloc_S);
			opCodes ["ldc.i4.s"] = new ILToken (Token.INSTR_I, Cecil.Cil.OpCodes.Ldc_I4_S);
			opCodes ["ldc.i4"] = new ILToken (Token.INSTR_I, Cecil.Cil.OpCodes.Ldc_I4);
			opCodes ["unaligned."] = new ILToken (Token.INSTR_I, Cecil.Cil.OpCodes.Unaligned);
			opCodes ["cpobj"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Cpobj);
			opCodes ["ldobj"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Ldobj);
			opCodes ["castclass"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Castclass);
			opCodes ["isinst"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Isinst);
			opCodes ["unbox"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Unbox);
			opCodes ["unbox.any"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Unbox_Any);
			opCodes ["stobj"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Stobj);
			opCodes ["box"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Box);
			opCodes ["newarr"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Newarr);
			opCodes ["ldelema"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Ldelema);
			opCodes ["refanyval"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Refanyval);
			opCodes ["mkrefany"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Mkrefany);
			opCodes ["initobj"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Initobj);
			opCodes ["sizeof"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Sizeof);
			opCodes ["stelem"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Stelem_Any);
			opCodes ["ldelem"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Ldelem_Any);
			opCodes ["stelem.any"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Stelem_Any);
			opCodes ["ldelem.any"] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Ldelem_Any);
			opCodes ["constrained."] = new ILToken (Token.INSTR_TYPE, Cecil.Cil.OpCodes.Constrained);
			opCodes ["jmp"] = new ILToken (Token.INSTR_METHOD, Cecil.Cil.OpCodes.Jmp);
			opCodes ["call"] = new ILToken (Token.INSTR_METHOD, Cecil.Cil.OpCodes.Call);
			opCodes ["callvirt"] = new ILToken (Token.INSTR_METHOD, Cecil.Cil.OpCodes.Callvirt);
			opCodes ["newobj"] = new ILToken (Token.INSTR_METHOD, Cecil.Cil.OpCodes.Newobj);
			opCodes ["ldftn"] = new ILToken (Token.INSTR_METHOD, Cecil.Cil.OpCodes.Ldftn);
			opCodes ["ldvirtftn"] = new ILToken (Token.INSTR_METHOD, Cecil.Cil.OpCodes.Ldvirtftn);
			opCodes ["ldfld"] = new ILToken (Token.INSTR_FIELD, Cecil.Cil.OpCodes.Ldfld);
			opCodes ["ldflda"] = new ILToken (Token.INSTR_FIELD, Cecil.Cil.OpCodes.Ldflda);
			opCodes ["stfld"] = new ILToken (Token.INSTR_FIELD, Cecil.Cil.OpCodes.Stfld);
			opCodes ["ldsfld"] = new ILToken (Token.INSTR_FIELD, Cecil.Cil.OpCodes.Ldsfld);
			opCodes ["ldsflda"] = new ILToken (Token.INSTR_FIELD, Cecil.Cil.OpCodes.Ldsflda);
			opCodes ["stsfld"] = new ILToken (Token.INSTR_FIELD, Cecil.Cil.OpCodes.Stsfld);
			opCodes ["br"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Br);
			opCodes ["brfalse"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Brfalse);
			opCodes ["brzero"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Brfalse);
			opCodes ["brnull"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Brfalse);
			opCodes ["brtrue"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Brtrue);
			opCodes ["beq"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Beq);
			opCodes ["bge"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bge);
			opCodes ["bgt"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bgt);
			opCodes ["ble"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Ble);
			opCodes ["blt"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Blt);
			opCodes ["bne.un"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bne_Un);
			opCodes ["bge.un"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bge_Un);
			opCodes ["bgt.un"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bgt_Un);
			opCodes ["ble.un"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Ble_Un);
			opCodes ["blt.un"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Blt_Un);
			opCodes ["leave"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Leave);
			opCodes ["br.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Br_S);
			opCodes ["brfalse.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Brfalse_S);
			opCodes ["brtrue.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Brtrue_S);
			opCodes ["beq.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Beq_S);
			opCodes ["bge.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bge_S);
			opCodes ["bgt.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bgt_S);
			opCodes ["ble.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Ble_S);
			opCodes ["blt.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Blt_S);
			opCodes ["bne.un.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bne_Un_S);
			opCodes ["bge.un.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bge_Un_S);
			opCodes ["bgt.un.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Bgt_Un_S);
			opCodes ["ble.un.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Ble_Un_S);
			opCodes ["blt.un.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Blt_Un_S);
			opCodes ["leave.s"] = new ILToken (Token.INSTR_BRTARGET, Cecil.Cil.OpCodes.Leave_S);
			opCodes ["ldstr"] = new ILToken (Token.INSTR_STRING, Cecil.Cil.OpCodes.Ldstr);
			opCodes ["ldc.r4"] = new ILToken (Token.INSTR_R, Cecil.Cil.OpCodes.Ldc_R4);
			opCodes ["ldc.r8"] = new ILToken (Token.INSTR_R, Cecil.Cil.OpCodes.Ldc_R8);
			opCodes ["ldc.i8"] = new ILToken (Token.INSTR_I8, Cecil.Cil.OpCodes.Ldc_I8);
			opCodes ["switch"] = new ILToken (Token.INSTR_SWITCH, Cecil.Cil.OpCodes.Switch);
			opCodes ["calli"] = new ILToken (Token.INSTR_SIG, Cecil.Cil.OpCodes.Calli);
			opCodes ["ldtoken"] = new ILToken (Token.INSTR_TOK, Cecil.Cil.OpCodes.Ldtoken);
			
			OpCodes = new ReadOnlyDictionaryAdapter<string, ILToken> (opCodes);
		}

		public static ReadOnlyDictionaryAdapter<string, ILToken> Directives { get; private set; }

		public static ReadOnlyDictionaryAdapter<string, ILToken> Keywords { get; private set; }
		
		public static ReadOnlyDictionaryAdapter<string, ILToken> OpCodes { get; private set; }
	}
}
