/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

#ifndef __MONO_ATOMIC_H__
#define __MONO_ATOMIC_H__

/*
 * This is the new internal atomics API for the Mono runtime. It is mostly
 * based on the C++11 memory model, with the addition of a 'strong' memory
 * order variant that corresponds to the __sync_* builtins provided by
 * GCC/Clang.
 *
 * Warning:
 *
 * 	On some 32-bit platforms, the 64-bit variants of the API will be
 * 	implemented using locks due to compiler, libc, or kernel limitations. In
 * 	those cases, they will be async-signal-unsafe. As such, it is a bad idea to
 * 	use 64-bit atomics in such contexts unless you know for certain that the
 * 	code will only run on 64-bit systems.
 *
 * 	For the same reason, be aware that atomic accesses to the same location but
 * 	with different bit widths (e.g. 32-bit and 64-bit) will not necessarily be
 * 	atomic with respect to each other due to the 64-bit case potentially doing
 * 	synchronization with a lock which the 32-bit case will not. Writing such
 * 	code is inadvisable in the first place, though.
 *
 * API:
 *
 * 	typedef enum {
 * 		MONO_ATOMIC_RELAX = ...,
 * 		MONO_ATOMIC_ACQUIRE = ...,
 * 		MONO_ATOMIC_RELEASE = ...,
 * 		MONO_ATOMIC_SEQCST = ...,
 * 		MONO_ATOMIC_STRONG = ...,
 * 	} MonoMemoryOrder;
 *
 * 	void mono_atomic_compiler_fence (MonoMemoryOrder order);
 * 	void mono_atomic_thread_fence (MonoMemoryOrder order);
 * 	void mono_atomic_process_fence (void);
 *
 * 	<type> mono_atomic_load_<type> (<type> *target, MonoMemoryOrder order);
 * 	void mono_atomic_store_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 *
 * 	<type> mono_atomic_swap_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_cas_<type> (<type> *target, <type> value, <type> comparand, MonoMemoryOrder order);
 *
 * 	<type> mono_atomic_fetch_add_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_sub_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_and_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_or_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_xor_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 *
 * 	<type> mono_atomic_add_fetch_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_sub_fetch_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_and_fetch_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_or_fetch_type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_xor_fetch_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 *
 * 	<type> mono_atomic_inc_<type> (<type> *target, MonoMemoryOrder order);
 * 	<type> mono_atomic_dec_<type> (<type> *target, MonoMemoryOrder order);
 *
 * Notes:
 *
 * - The exact values of the MonoMemoryOrder enumeration depend on the
 *   platform. Do not rely on them being the same across systems.
 * - It is unspecified whether the API is implemented as macros or functions,
 *   and this can differ based on platform details. Do not try to take the
 *   address of any of these.
 * - For the sake of simplicity, mono_atomic_cas_* does not take a failure
 *   order parameter, and instead uses the order parameter for both the success
 *   and failure case. There is also no support for 'weak' CAS operations.
 * - There is deliberately no support for the 'consume' memory order as it is
 *   widely considered undercooked and confusing, and is not really supported
 *   in a meaningful way by GCC/Clang anyway. You should probably be using
 *   MONO_ATOMIC_ACQUIRE instead.
 * - There is full API coverage for all integer types. gpointer/ptr and
 *   gboolean/bool do not support read-modify-write (fetch) and
 *   increment/decrement operations.
 */

#include <mono/utils/mono-compiler.h>
#include <glib.h>

/*
 * Some platforms (usually 32-bit ones) have broken 64-bit builtins. These are
 * sometimes impossible to detect at compile time (e.g. Android/ARM). Use the
 * lock-based fallbacks for 64-bit atomics in these cases.
 */
#if (defined (HAVE_ATOMIC_BUILTINS) || defined (HAVE_SYNC_BUILTINS)) && \
    (SIZEOF_VOID_P == 4 && (defined (HOST_ARM) || defined (HOST_MIPS) || defined (HOST_POWERPC)))
#	define _MONO_ATOMIC_BROKEN_64
#endif

void
mono_atomic_process_fence (void);

#define _MONO_ATOMIC_ATTRS static inline MONO_ALWAYS_INLINE

/*
 * The platform-specific headers are expected to implement the following:
 *
 * 	void mono_atomic_compiler_fence (MonoMemoryOrder order);
 * 	void mono_atomic_thread_fence (MonoMemoryOrder order);
 *
 * 	<type> mono_atomic_load_<type> (<type> *target, MonoMemoryOrder order);
 * 	void mono_atomic_store_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 *
 * 	<type> mono_atomic_swap_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_cas_<type> (<type> *target, <type> value, <type> comparand, MonoMemoryOrder order);
 *
 * 	<type> mono_atomic_fetch_add_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_sub_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_and_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_or_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 * 	<type> mono_atomic_fetch_xor_<type> (<type> *target, <type> value, MonoMemoryOrder order);
 *
 * These must be implemented for gint8/i8, gint16/i16, and gint32/i32. They
 * should only be implemented for gint64/i64 if _MONO_ATOMIC_BROKEN_64 is not
 * defined (see comment above).
 */
#if defined (HOST_WIN32)
#	include <mono/utils/mono-atomic-windows.h>
#elif defined (HAVE_ATOMIC_BUILTINS)
#	include <mono/utils/mono-atomic-new.h>
#elif defined (HAVE_SYNC_BUILTINS)
#	include <mono/utils/mono-atomic-old.h>
#else
#	include <mono/utils/mono-atomic-fallback.h>
#endif

#if defined (_MONO_ATOMIC_BROKEN_64)
#	include <mono/utils/mono-atomic-fallback.h>
#endif

/*
 * Implement the gsize variants of the API in terms of the gint32/gint64
 * variants defined in the platform-specific headers depending on bitness.
 */

#if SIZEOF_VOID_P == 8
#	define ATOMIC_WORD_NAME(name) name ## i64
#	define ATOMIC_WORD_TYPE gint64
#else
#	define ATOMIC_WORD_NAME(name) name ## i32
#	define ATOMIC_WORD_TYPE gint32
#endif

_MONO_ATOMIC_ATTRS gsize
mono_atomic_load_word (gsize *target, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_load_) ((ATOMIC_WORD_TYPE *) target, order);
}

_MONO_ATOMIC_ATTRS void
mono_atomic_store_word (gsize *target, gsize value, MonoMemoryOrder order)
{
	ATOMIC_WORD_NAME (mono_atomic_store_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, order);
}

_MONO_ATOMIC_ATTRS gsize
mono_atomic_swap_word (gsize *target, gsize value, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_swap_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, order);
}

_MONO_ATOMIC_ATTRS gsize
mono_atomic_cas_word (gsize *target, gsize value, gsize comparand, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_cas_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, (ATOMIC_WORD_TYPE) comparand, order);
}

_MONO_ATOMIC_ATTRS gsize
mono_atomic_fetch_add_word (gsize *target, gsize value, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_fetch_add_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, order);
}

_MONO_ATOMIC_ATTRS gsize
mono_atomic_fetch_sub_word (gsize *target, gsize value, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_fetch_sub_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, order);
}

_MONO_ATOMIC_ATTRS gsize
mono_atomic_fetch_and_word (gsize *target, gsize value, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_fetch_and_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, order);
}

_MONO_ATOMIC_ATTRS gsize
mono_atomic_fetch_or_word (gsize *target, gsize value, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_fetch_or_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, order);
}

_MONO_ATOMIC_ATTRS gsize
mono_atomic_fetch_xor_word (gsize *target, gsize value, MonoMemoryOrder order)
{
	return (gsize) ATOMIC_WORD_NAME (mono_atomic_fetch_xor_) ((ATOMIC_WORD_TYPE *) target, (ATOMIC_WORD_TYPE) value, order);
}

#undef ATOMIC_WORD_NAME
#undef ATOMIC_WORD_TYPE

/*
 * Implement the gpointer/ptr variants of the API in terms of the gsize/word
 * variants defined above.
 */

_MONO_ATOMIC_ATTRS gpointer
mono_atomic_load_ptr (gpointer *target, MonoMemoryOrder order)
{
	return (gpointer) mono_atomic_load_word ((gsize *) target, order);
}

_MONO_ATOMIC_ATTRS void
mono_atomic_store_ptr (gpointer *target, gpointer value, MonoMemoryOrder order)
{
	mono_atomic_store_word ((gsize *) target, (gsize) value, order);
}

_MONO_ATOMIC_ATTRS gpointer
mono_atomic_swap_ptr (gpointer *target, gpointer value, MonoMemoryOrder order)
{
	return (gpointer) mono_atomic_swap_word ((gsize *) target, (gsize) value, order);
}

_MONO_ATOMIC_ATTRS gpointer
mono_atomic_cas_ptr (gpointer *target, gpointer value, gpointer comparand, MonoMemoryOrder order)
{
	return (gpointer) mono_atomic_cas_word ((gsize *) target, (gsize) value, (gsize) comparand, order);
}

/*
 * Implement the gboolean/bool variants of the API in terms of the gint32/i32
 * variants defined in the platform-specific headers.
 */

_MONO_ATOMIC_ATTRS gboolean
mono_atomic_load_bool (gboolean *target, MonoMemoryOrder order)
{
	return (gboolean) mono_atomic_load_i32 ((gint32 *) target, order);
}

_MONO_ATOMIC_ATTRS void
mono_atomic_store_bool (gboolean *target, gboolean value, MonoMemoryOrder order)
{
	mono_atomic_store_i32 ((gint32 *) target, (gint32) value, order);
}

_MONO_ATOMIC_ATTRS gboolean
mono_atomic_swap_bool (gboolean *target, gboolean value, MonoMemoryOrder order)
{
	return (gboolean) mono_atomic_swap_i32 ((gint32 *) target, (gint32) value, order);
}

_MONO_ATOMIC_ATTRS gboolean
mono_atomic_cas_bool (gboolean *target, gboolean value, gboolean comparand, MonoMemoryOrder order)
{
	return (gboolean) mono_atomic_cas_i32 ((gint32 *) target, (gint32) value, (gint32) comparand, order);
}

/*
 * Implement the <op>-fetch variants of the API in terms of the fetch-<op>
 * variants defined in the platform-specific headers.
 */

#define ATOMIC_OP_FETCH(type, tname, op, oname) \
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_ ## oname ## _fetch_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		return mono_atomic_fetch_ ## oname ## _ ## tname (target, value, order) op value; \
	}

ATOMIC_OP_FETCH (gint8, i8, +, add)
ATOMIC_OP_FETCH (gint8, i8, -, sub)
ATOMIC_OP_FETCH (gint8, i8, &, and)
ATOMIC_OP_FETCH (gint8, i8, |, or)
ATOMIC_OP_FETCH (gint8, i8, ^, xor)

ATOMIC_OP_FETCH (gint16, i16, +, add)
ATOMIC_OP_FETCH (gint16, i16, -, sub)
ATOMIC_OP_FETCH (gint16, i16, &, and)
ATOMIC_OP_FETCH (gint16, i16, |, or)
ATOMIC_OP_FETCH (gint16, i16, ^, xor)

ATOMIC_OP_FETCH (gint32, i32, +, add)
ATOMIC_OP_FETCH (gint32, i32, -, sub)
ATOMIC_OP_FETCH (gint32, i32, &, and)
ATOMIC_OP_FETCH (gint32, i32, |, or)
ATOMIC_OP_FETCH (gint32, i32, ^, xor)

ATOMIC_OP_FETCH (gint64, i64, +, add)
ATOMIC_OP_FETCH (gint64, i64, -, sub)
ATOMIC_OP_FETCH (gint64, i64, &, and)
ATOMIC_OP_FETCH (gint64, i64, |, or)
ATOMIC_OP_FETCH (gint64, i64, ^, xor)

ATOMIC_OP_FETCH (gsize, word, +, add)
ATOMIC_OP_FETCH (gsize, word, -, sub)
ATOMIC_OP_FETCH (gsize, word, &, and)
ATOMIC_OP_FETCH (gsize, word, |, or)
ATOMIC_OP_FETCH (gsize, word, ^, xor)

#undef ATOMIC_OP_FETCH

/*
 * Implement the increment/decrement APIs in terms of the <op>-fetch APIs
 * defined above.
 */

#define ATOMIC_OP_INCDEC(type, tname) \
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_inc_ ## tname (type *target, MonoMemoryOrder order) \
	{ \
		return mono_atomic_add_fetch_ ## tname (target, 1, order) + 1; \
	} \
	\
	_MONO_ATOMIC_ATTRS type \
	mono_atomic_dec_ ## tname (type *target, MonoMemoryOrder order) \
	{ \
		return mono_atomic_sub_fetch_ ## tname (target, 1, order) - 1; \
	}

ATOMIC_OP_INCDEC (gint8, i8)
ATOMIC_OP_INCDEC (gint16, i16)
ATOMIC_OP_INCDEC (gint32, i32)
ATOMIC_OP_INCDEC (gint64, i64)
ATOMIC_OP_INCDEC (gsize, word)

#undef ATOMIC_OP_INCDEC

#endif /* __MONO_ATOMIC_H__ */
