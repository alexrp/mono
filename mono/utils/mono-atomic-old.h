/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

/*
 * __sync-based implementation of the atomics API. Do not include directly.
 *
 * We fall back to the legacy __sync_* builtins. These have no concept of
 * memory order and always perform full fences, i.e. they behave as if you
 * always pass MONO_ATOMIC_STRONG (except for mono_atomic_thread_fence). They
 * also have no way of doing standalone atomic loads and stores, so we have to
 * emulate those (e.g. with a CAS loop). This is all quite inefficient, of
 * course, but at least correct. The good news is that most compiler versions
 * in use today will not be using this code.
 */

typedef enum {
	MONO_ATOMIC_RELAX = 0,
	MONO_ATOMIC_ACQUIRE = 1,
	MONO_ATOMIC_RELEASE = 2,
	MONO_ATOMIC_SEQCST = 3,
	MONO_ATOMIC_STRONG = 4,
} MonoMemoryOrder;

_MONO_ATOMIC_ATTRS void
mono_atomic_compiler_fence (MonoMemoryOrder order)
{
	if (order != MONO_ATOMIC_RELAX)
		asm volatile ("" : : : "memory");
}

_MONO_ATOMIC_ATTRS void
mono_atomic_thread_fence (MonoMemoryOrder order)
{
	if (order != MONO_ATOMIC_RELAX)
		__sync_synchronize ();
}

/*
 * Some versions of GCC and all current versions of Clang get fences around
 * the old __sync_* builtins wrong on ARMv8. We work around this by inserting
 * fences manually if we are built with a broken compiler. This is only
 * relevant if MONO_ATOMIC_STRONG is passed.
 *
 * Context:
 *
 * - http://lists.infradead.org/pipermail/linux-arm-kernel/2014-February/229588.html
 * - https://bugs.llvm.org/show_bug.cgi?id=29102
 */
#if __ARM_ARCH == 8 && (defined (__clang__) || MONO_GNUC_VERSION < 50300)
#	define ATOMIC_STRONG_FENCE(order) \
	do { \
		if ((order) == MONO_ATOMIC_STRONG) \
			mono_atomic_thread_fence ((order)); \
	} while (0)
#else
#	define ATOMIC_STRONG_FENCE(order)
#endif

#define ATOMIC_OLD_IMPL(type, tname) \
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_cas_ ## tname (type *target, type value, type comparand, MonoMemoryOrder order) \
	{ \
		type result = __sync_val_compare_and_swap (target, comparand, value); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_add_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __sync_fetch_and_add (target, value); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_sub_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __sync_fetch_and_sub (target, value); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_and_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __sync_fetch_and_and (target, value); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_or_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __sync_fetch_and_or (target, value); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_xor_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __sync_fetch_and_xor (target, value); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_load_ ## tname (type *target, MonoMemoryOrder order) \
	{ \
		ATOMIC_STRONG_FENCE (order); \
		return mono_atomic_fetch_add_ ## tname (target, 0, order); \
	} \
	\
	_MONO_ATOMIC_ATTRS void \
	mono_atomic_store_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type previous; \
		do { \
			previous = mono_atomic_load_ ## tname (target, MONO_ATOMIC_RELAX); \
		} while (mono_atomic_cas_ ## tname (target, previous, value, order) != previous); \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_swap_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type previous; \
		do { \
			previous = mono_atomic_load_ ## tname (target, MONO_ATOMIC_RELAX); \
		} while (mono_atomic_cas_ ## tname (target, previous, value, order) != previous); \
		return previous; \
	}

ATOMIC_OLD_IMPL (gint8, i8)
ATOMIC_OLD_IMPL (gint16, i16)
ATOMIC_OLD_IMPL (gint32, i32)

#if !defined (_MONO_ATOMIC_BROKEN_64)

ATOMIC_OLD_IMPL (gint64, i64)

#endif

#undef ATOMIC_OLD_IMPL
#undef ATOMIC_STRONG_FENCE
