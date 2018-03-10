// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Mono.Profiler.Log {

	public struct LogBacktraceFrame {

		public long MethodPointer { get; internal set; }

		public long ILOffset { get; internal set; }
	}
}
