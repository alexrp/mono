/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

/*
 * Windows implementation of the atomics API. Do not include directly.
 *
 * Newer versions of MSVC have a ton of intrinsics for implementing the C++11
 * memory model. We can make use of these just like the <atomic> header in
 * MSVC's STL implementation does. This is convenient since the Interlocked*
 * functions declared in <winnt.h> can lag behind the intrinsics in type and
 * fence kind coverage.
 */

#include <intrin.h>

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
		_ReadWriteBarrier ();
}

_MONO_ATOMIC_ATTRS void
mono_atomic_thread_fence (MonoMemoryOrder order)
{
	/*
	 * These intrinsics have the semantics of an architecture-level fence plus
	 * an implicit call to _ReadWriteBarrier (compiler fence).
	 */
	if (order != MONO_ATOMIC_RELAX)
#if defined (HOST_ARM)
		__dmb (_ARM_BARRIER_ISH);
#elif defined (HOST_ARM64)
		__dmb (_ARM64_BARRIER_ISH);
#else
		_mm_mfence ();
#endif
}

_MONO_ATOMIC_ATTRS gint8
mono_atomic_load_i8 (gint8 *target, MonoMemoryOrder order)
{
}

_MONO_ATOMIC_ATTRS gint16
mono_atomic_load_i16 (gint16 *target, MonoMemoryOrder order)
{
}

_MONO_ATOMIC_ATTRS gint32
mono_atomic_load_i32 (gint32 *target, MonoMemoryOrder order)
{
}

_MONO_ATOMIC_ATTRS gint64
mono_atomic_load_i64 (gint64 *target, MonoMemoryOrder order)
{
}

_MONO_ATOMIC_ATTRS void
mono_atomic_store_i8 (gint8 *target, gint8 value, MonoMemoryOrder order)
{
}

_MONO_ATOMIC_ATTRS void
mono_atomic_store_i16 (gint16 *target, gint16 value, MonoMemoryOrder order)
{
}

_MONO_ATOMIC_ATTRS void
mono_atomic_store_i32 (gint32 *target, gint32 value, MonoMemoryOrder order)
{
}

_MONO_ATOMIC_ATTRS void
mono_atomic_store_i64 (gint64 *target, gint64 value, MonoMemoryOrder order)
{
}
