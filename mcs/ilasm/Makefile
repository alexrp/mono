thisdir = ilasm
include ../build/rules.make

PROGRAM = ilasm.exe
BUILT_SOURCES = ILParser.cs
LOCAL_MCS_FLAGS = /keyfile:../class/mono.snk /lib:../class/lib/$(PROFILE) /r:Mono.Cecil.dll /r:Mono.Cecil.Mdb.dll /r:Mono.Security.dll

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

run-test: $(PROGRAM)
	find tests -name "*.cs" > tests/tests.dll.sources
	$(MCS) /debug+ /define:"DEBUG;TRACE;NET_4_0" /target:library /keyfile:../class/mono.snk /out:tests/bin/Debug/tests.dll /r:Mono.Cecil.dll /r:nunit.framework.dll @tests/tests.dll.sources ../build/common/Consts.cs
	cp ../class/lib/$(PROFILE)/ilasm.exe tests/bin/Debug/ilasm.exe
	nunit-console ilasm.nunit
