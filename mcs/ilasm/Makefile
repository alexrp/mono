thisdir = ilasm
include ../build/rules.make

PROGRAM = ilasm.exe
BUILT_SOURCES = ILParser.cs
LOCAL_MCS_FLAGS = /keyfile:../class/mono.snk /lib:../class/lib/$(PROFILE) /r:Mono.Cecil.dll /r:Mono.Cecil.Mdb.dll /r:Mono.Security.dll

EXTRA_DISTFILES = \
	parser/ChangeLog	\
	scanner/ChangeLog	\
	tests/ChangeLog		\

ILParser.cs: parser/ILParser.jay $(topdir)/jay/skeleton.cs
	$(topdir)/jay/jay -ct < $(topdir)/jay/skeleton.cs $(CURDIR)/$< >$@

jay: ILParser.cs

include ../build/executable.make

run-test: $(PROGRAM)
	find tests -name "*.cs" > tests/tests.dll.sources
	if ! test -d tests/bin/Debug; then mkdir tests/bin && mkdir tests/bin/Debug; fi
	$(MCS) /debug+ /define:"DEBUG;TRACE;NET_2_0" /target:library /keyfile:../class/mono.snk /out:tests/bin/Debug/tests.dll /r:ilasm.exe /r:Mono.Cecil.dll /r:Mono.Cecil.Mdb.dll /r:nunit.framework.dll @tests/tests.dll.sources ../build/common/Consts.cs
	cp ../class/lib/$(PROFILE)/ilasm.* tests/bin/Debug
	nunit-console ilasm.nunit
