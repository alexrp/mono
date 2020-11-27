/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

#include <mono/utils/mono-atomic.h>
#include <mono/utils/mono-once.h>
#include <mono/utils/mono-os-mutex.h>
#if defined (HAVE_SYS_MMAN_H)
#	include <sys/mman.h>
#endif
#if defined (HAVE_SYS_SYSCALL_H)
#	include <sys/syscall.h>
#endif

#if defined (HOST_WIN32)

void
mono_atomic_process_fence (void)
{
	FlushProcessWriteBuffers ();
}

#elif defined (HOST_WASM)

void
mono_atomic_process_fence (void)
{
}

#else

#if defined (SYS_membarrier)

/*
 * Values taken from <linux/membarrier.h>. Copied here to avoid relying on a
 * specific version of that header. Values are guaranteed to be stable.
 */

#define MEMBARRIER_CMD_QUERY (0)
#define MEMBARRIER_CMD_GLOBAL (1 << 0)
#define MEMBARRIER_CMD_GLOBAL_EXPEDITED (1 << 1)
#define MEMBARRIER_CMD_REGISTER_GLOBAL_EXPEDITED (1 << 2)
#define MEMBARRIER_CMD_PRIVATE_EXPEDITED (1 << 3)
#define MEMBARRIER_CMD_REGISTER_PRIVATE_EXPEDITED (1 << 4)
#define MEMBARRIER_CMD_PRIVATE_EXPEDITED_SYNC_CORE (1 << 5)
#define MEMBARRIER_CMD_REGISTER_PRIVATE_EXPEDITED_SYNC_CORE (1 << 6)

static int
membarrier (int cmd)
{
	return syscall (SYS_membarrier, cmd, 0 /* flags */);
}

static gboolean sys_membarrier_global;
static gboolean sys_membarrier_local;

#endif

static mono_once_t process_fence_once = MONO_ONCE_INIT;
static mono_mutex_t process_fence_mutex;
static void *process_fence_page;

static void
process_fence_init (void)
{
#if defined (SYS_membarrier)
	int mask = membarrier (MEMBARRIER_CMD_QUERY);

	if (mask != -1) {
		if (mask & MEMBARRIER_CMD_GLOBAL)
			sys_membarrier_global = TRUE;

		if (!membarrier (MEMBARRIER_CMD_REGISTER_PRIVATE_EXPEDITED))
			sys_membarrier_local = TRUE;
	}
#endif

	mono_os_mutex_init (&process_fence_mutex);
	g_assert (!posix_memalign (&process_fence_page, PAGESIZE, PAGESIZE));
}

#endif

void
mono_atomic_process_fence (void)
{
	mono_once (&process_fence_once, &process_fence_init);

#	if defined (SYS_membarrier)

	/*
	 * Try the membarrier system call first as it should be faster.
	 */

	if (sys_membarrier_local && !membarrier (MEMBARRIER_CMD_PRIVATE_EXPEDITED))
		return;

	if (sys_membarrier_global && !membarrier (MEMBARRIER_CMD_GLOBAL))
		return;

#	endif

	mono_os_mutex_lock (&process_fence_mutex);

	/*
	 * Changing a helper memory page protection from read/write to no access
	 * causes the OS to issue an IPI to flush TLBs on all processors. This also
	 * results in flushing the processor buffers.
	 *
	 * We write to the page to ensure that it is dirty before we change the
	 * protection to no access, so that we prevent the OS from skipping the
	 * global TLB flush.
	 */
	g_assert (!mprotect (process_fence_page, PAGESIZE, PROT_READ | PROT_WRITE));
	mono_atomic_store_word ((gsize *) process_fence_page, 1, MONO_ATOMIC_STRONG);
	g_assert (!mprotect (process_fence_page, PAGESIZE, PROT_NONE));

	mono_os_mutex_unlock (&process_fence_mutex);
}

#endif
