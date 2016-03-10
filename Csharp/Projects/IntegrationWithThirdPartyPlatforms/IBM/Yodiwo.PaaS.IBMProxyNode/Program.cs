using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yodiwo.PaaS.IBMProxyNode
{
    class Program
    {
        public static void Main()
        {
            IBMProxy proxy = new IBMProxy();
            proxy.Init();
            while (true)
                Task.Delay(200).Wait();

        }
    }
}
