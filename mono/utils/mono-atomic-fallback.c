/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

#include <mono/utils/mono-atomic.h>
#include <mono/utils/mono-once.h>
#include <mono/utils/mono-os-mutex.h>

#if defined (_MONO_ATOMIC_FALLBACK) || defined (_MONO_ATOMIC_BROKEN_64)

static mono_once_t fallback_once = MONO_ONCE_INIT;
static mono_mutex_t fallback_mutex;

static void
fallback_init (void)
{
	mono_os_mutex_init (&fallback_mutex);
}

#endif

#define ATOMIC_FALLBACK_BEGIN() \
	do { \
		mono_once (&fallback_once, &fallback_init); \
		mono_os_mutex_lock (&fallback_mutex); \
	} while (0)

#define ATOMIC_FALLBACK_END() \
	do { \
		mono_os_mutex_unlock (&fallback_mutex); \
	} while (0)

#define ATOMIC_FALLBACK_IMPL(type, tname) \
	type \
	mono_atomic_load_ ## tname (type *target, MonoMemoryOrder order)
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type result = *target; \
		ATOMIC_FALLBACK_END (); \
		return result; \
	} \
	\
	void \
	mono_atomic_store_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		*target = value;
		ATOMIC_FALLBACK_END (); \
	} \
	\
	type \
	mono_atomic_swap_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type result = *target; \
		*target = value; \
		ATOMIC_FALLBACK_END (); \
		return result; \
	} \
	\
	type \
	mono_atomic_cas_ ## tname (type *target, type value, type comparand, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type previous = *target; \
		if (previous == comparand) \
			*target = value; \
		ATOMIC_FALLBACK_END (); \
		return previous; \
	} \
	\
	type \
	mono_atomic_fetch_add_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type previous = *target; \
		*target += value; \
		ATOMIC_FALLBACK_END (); \
		return previous; \
	} \
	\
	type \
	mono_atomic_fetch_sub_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type previous = *target; \
		*target -= value; \
		ATOMIC_FALLBACK_END (); \
		return previous; \
	} \
	\
	type \
	mono_atomic_fetch_and_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type previous = *target; \
		*target &= value; \
		ATOMIC_FALLBACK_END (); \
		return previous; \
	} \
	\
	type \
	mono_atomic_fetch_or_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type previous = *target; \
		*target |= value; \
		ATOMIC_FALLBACK_END (); \
		return previous; \
	} \
	\
	type \
	mono_atomic_fetch_xor_ ## tname (type *target, type value, MonoMemoryOrder order) \
	{ \
		ATOMIC_FALLBACK_BEGIN (); \
		type previous = *target; \
		*target ^= value; \
		ATOMIC_FALLBACK_END (); \
		return previous; \
	}

#if defined (_MONO_ATOMIC_FALLBACK)

void
mono_atomic_thread_fence (MonoMemoryOrder order)
{
	/*
	 * This should be sufficient for weird systems that actually hit this path.
	 */
	mono_atomic_compiler_fence (MONO_ATOMIC_STRONG);
	ATOMIC_FALLBACK_BEGIN;
	ATOMIC_FALLBACK_END;
	mono_atomic_compiler_fence (MONO_ATOMIC_STRONG);
}

ATOMIC_FALLBACK_IMPL (gint8, i8)
ATOMIC_FALLBACK_IMPL (gint16, i16)
ATOMIC_FALLBACK_IMPL (gint32, i32)

#endif

#if defined (_MONO_ATOMIC_FALLBACK) || defined (_MONO_ATOMIC_BROKEN_64)

ATOMIC_FALLBACK_IMPL (gint64, i64)

#endif

#undef ATOMIC_FALLBACK_IMPL
#undef ATOMIC_FALLBACK_END
#undef ATOMIC_FALLBACK_BEGIN
