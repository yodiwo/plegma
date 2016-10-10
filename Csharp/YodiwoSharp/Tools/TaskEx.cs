using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static class TaskEx
    {
        public static void DelayWait(int ms)
        {
            Task.Delay(ms).Wait();
        }

#if VFSharp
        public static void DelayWait(TimespanEx delay)
        {
            Task.Delay((int)delay.MillisecondsI).Wait();
        }
#endif

        public static void DelayWait(TimeSpan delay)
        {
            Task.Delay(delay).Wait();
        }

        public static Task RunSafe(Action action, bool AssertException = true)
        {
            return Task.Run(() =>
            {
                try { action(); }
                catch (Exception ex) { if (AssertException) DebugEx.Assert(ex); }
            });
        }

        public static Task<T> RunSafe<T>(Func<T> action, bool AssertException = true, T Default = default(T))
        {
            return Task.Run(() =>
            {
                try { return action(); }
                catch (Exception ex)
                {
                    if (AssertException)
                        DebugEx.Assert(ex);
                    return Default;
                }
            });
        }
    }
}
