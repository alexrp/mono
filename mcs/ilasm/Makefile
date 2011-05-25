thisdir = ilasm
include ../build/rules.make

PROGRAM = ilasm.exe
BUILT_SOURCES = ILParser.cs
LOCAL_MCS_FLAGS = /lib:../class/lib/$(PROFILE) /r:../class/lib/$(PROFILE)/Mono.Cecil.dll /r:../class/lib/$(PROFILE)/Mono.Cecil.Mdb.dll /r:Mono.CompilerServices.SymbolWriter.dll /r:Mono.Security.dll

EXTRA_DISTFILES = \
	parser/ILParser.jay	\
	parser/ChangeLog	\
	scanner/ChangeLog	\
	tests/ChangeLog		\
	errors/ChangeLog	\
	$(wildcard tests/*.il)	\
	$(wildcard tests/*.c)	\
	$(wildcard tests/*.cs)	\
	$(wildcard tests/*.snk)	\
	$(wildcard errors/*.il)

ILParser.cs: parser/ILParser.jay $(topdir)/jay/skeleton.cs
	$(topdir)/jay/jay -ct < $(topdir)/jay/skeleton.cs $(CURDIR)/$< >$@

jay: ILParser.cs

include ../build/executable.make
