/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

/*
 * Fallback implementation of the atomics API. Do not include directly.
 *
 * Fall back to a lock-based implementation in mono-atomic-fallback.c. This
 * path is hit either on very ancient systems without compiler builtins or on
 * some platforms where 64-bit builtins are broken.
 */

#if !defined (_MONO_ATOMIC_BROKEN_64)
#	define _MONO_ATOMIC_FALLBACK
#endif

typedef enum {
	MONO_ATOMIC_RELAX = 0,
	MONO_ATOMIC_ACQUIRE = 0,
	MONO_ATOMIC_RELEASE = 0,
	MONO_ATOMIC_SEQCST = 0,
	MONO_ATOMIC_STRONG = 0,
} MonoMemoryOrder;

#define ATOMIC_FALLBACK_DECL(type, tname) \
	type \
	mono_atomic_load_ ## tname (type *target, MonoMemoryOrder order); \
	\
	void \
	mono_atomic_store_ ## tname (type *target, type value, MonoMemoryOrder order); \
	\
	type \
	mono_atomic_swap_ ## tname (type *target, type value, MonoMemoryOrder order); \
	\
	type \
	mono_atomic_cas_ ## tname (type *target, type value, type comparand, MonoMemoryOrder order); \
	\
	type \
	mono_atomic_fetch_add_ ## tname (type *target, type value, MonoMemoryOrder order); \
	\
	type \
	mono_atomic_fetch_sub_ ## tname (type *target, type value, MonoMemoryOrder order); \
	\
	type \
	mono_atomic_fetch_and_ ## tname (type *target, type value, MonoMemoryOrder order); \
	\
	type \
	mono_atomic_fetch_or_ ## tname (type *target, type value, MonoMemoryOrder order); \
	\
	type \
	mono_atomic_fetch_xor_ ## tname (type *target, type value, MonoMemoryOrder order);

#if defined (_MONO_ATOMIC_FALLBACK)

_MONO_ATOMIC_ATTRS void
mono_atomic_compiler_fence (MonoMemoryOrder order)
{
	if (order != MONO_ATOMIC_RELAX)
		asm volatile ("" : : : "memory");
}

void
mono_atomic_thread_fence (MonoMemoryOrder order);

ATOMIC_FALLBACK_DECL (gint8, i8)
ATOMIC_FALLBACK_DECL (gint16, i16)
ATOMIC_FALLBACK_DECL (gint32, i32)

#endif

ATOMIC_FALLBACK_DECL (gint64, i64)

#undef ATOMIC_FALLBACK_DECL
