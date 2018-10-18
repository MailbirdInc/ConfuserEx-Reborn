using System;
using System.Diagnostics;
using System.Threading;

namespace Confuser.Runtime {
	internal static class AntiDebugSafe {
		static void Initialize() {
            // Removed COR_ENABLE_PROFILING check here since it caused issues with Stackify Prefix which sets that variable.
            // I'm thinking technically this check should have also checked that COR_PROFILER was set to something:
            // https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/profiling/setting-up-a-profiling-environment

            //string x = "COR";
            //var env = typeof(Environment);
            //var method = env.GetMethod("GetEnvironmentVariable", new[] { typeof(string) });
            //if (method != null &&
            //    "1".Equals(method.Invoke(null, new object[] { x + "_ENABLE_PROFILING" })))
            //	Environment.FailFast(null);

            var thread = new Thread(Worker);
			thread.IsBackground = true;
			thread.Start(null);
		}

		static void Worker(object thread) {
			var th = thread as Thread;
			if (th == null) {
				th = new Thread(Worker);
				th.IsBackground = true;
				th.Start(Thread.CurrentThread);
				Thread.Sleep(500);
			}
			while (true) {
				if (Debugger.IsAttached || Debugger.IsLogging())
					Environment.FailFast(null);

				if (!th.IsAlive)
					Environment.FailFast(null);

				Thread.Sleep(1000);
			}
		}
	}
}