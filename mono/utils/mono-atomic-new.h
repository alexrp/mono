/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

/*
 * __atomic-based implementation of the atomics API. Do not include directly.
 *
 * This is the new and preferred implementation, based on the __atomic_*
 * builtins which implement the C++11 memory model. For the most part, we just
 * delegate to those builtins and call it a day. We only need to do some extra
 * work for MONO_ATOMIC_STRONG.
 *
 * It is important that all APIs in this implementation are either macros or
 * functions marked with _MONO_ATOMIC_ATTRS so that they are always inlined. If
 * this is not the case, the compiler will be unable to statically determine
 * the memory order passed to the builtins and will therefore emit code using
 * the strongest memory order, resulting in worse performance.
 */

/*
 * Figure out whether the strong memory order is supported by the compiler. If
 * not, we need to insert fences manually.
 *
 * GCC version 5.1 and later support requesting the strong __sync_* memory
 * order by passing __ATOMIC_SEQ_CST | (1 << 15) as the order value. See
 * gcc/memmodel.h in the GCC source tree for details. Clang does not support
 * this.
 */
#if !defined (__clang__) && MONO_GNUC_VERSION >= 50100
#	define ATOMIC_STRONG_VALUE (__ATOMIC_SEQ_CST | (1 << 15))
#	define ATOMIC_STRONG_FENCE(order)
#else
#	define ATOMIC_STRONG_FENCE(order) \
	do { \
		if ((order) == MONO_ATOMIC_STRONG) \
			mono_atomic_thread_fence ((order)); \
	} while (0)
#endif

typedef enum {
	MONO_ATOMIC_RELAX = __ATOMIC_RELAXED,
	MONO_ATOMIC_ACQUIRE = __ATOMIC_ACQUIRE,
	MONO_ATOMIC_RELEASE = __ATOMIC_RELEASE,
	MONO_ATOMIC_SEQCST = __ATOMIC_SEQ_CST,
#if defined (ATOMIC_STRONG_VALUE)
	MONO_ATOMIC_STRONG = ATOMIC_STRONG_VALUE,
#	undef ATOMIC_STRONG_VALUE
#else
	MONO_ATOMIC_STRONG = MONO_ATOMIC_SEQCST,
#endif
} MonoMemoryOrder;

_MONO_ATOMIC_ATTRS void
mono_atomic_compiler_fence (MonoMemoryOrder order)
{
	__atomic_signal_fence (order);
}

_MONO_ATOMIC_ATTRS void
mono_atomic_thread_fence (MonoMemoryOrder order)
{
	/*
	 * For explicit fences, MONO_ATOMIC_SEQCST has the same semantics as
	 * MONO_ATOMIC_STRONG, so no need to do anything special here.
	 */
	__atomic_thread_fence (order);
}

#define ATOMIC_NEW_IMPL(type, tname) \
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_load_ ## tname (type *target, MonoMemoryOrder order) \
	{ \
		ATOMIC_STRONG_FENCE (order); \
		return __atomic_load_n (target, order); \
	} \
	\
	_MONO_ATOMIC_ATTRS void \
	mono_atomic_store_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		__atomic_store_n (target, value, order); \
		ATOMIC_STRONG_FENCE (order); \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_swap_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type previous = __atomic_exchange_n (target, value, order); \
		ATOMIC_STRONG_FENCE (order); \
		return previous; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_cas_ ## tname (type *target, type value, type comparand, MonoMemoryOrder order) \
	{ \
		type previous = __atomic_compare_exchange_n (target, &comparand, value, FALSE, order, order); \
		ATOMIC_STRONG_FENCE (order); \
		return previous; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_add_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __atomic_fetch_add (target, value, order); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_sub_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __atomic_fetch_sub (target, value, order); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_and_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __atomic_fetch_and (target, value, order); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_or_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __atomic_fetch_or (target, value, order); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_fetch_xor_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		type result = __atomic_fetch_xor (target, value, order); \
		ATOMIC_STRONG_FENCE (order); \
		return result; \
	}

ATOMIC_NEW_IMPL (gint8, i8)
ATOMIC_NEW_IMPL (gint16, i16)
ATOMIC_NEW_IMPL (gint32, i32)

#if !defined (_MONO_ATOMIC_BROKEN_64)

ATOMIC_NEW_IMPL (gint64, i64)

#endif

#undef ATOMIC_NEW_IMPL
#undef ATOMIC_STRONG_FENCE
