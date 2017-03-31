using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public class Thread
    {
        public static void Sleep(int ms)
        {
            Task.Delay(ms).Wait();
        }
        public static void Sleep(TimeSpan delay)
        {
            Task.Delay(delay).Wait();
        }
    }
}
