using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yodiwo.PaaS.AzureProxyNode
{
    class Program
    {
        static void Main(string[] args)
        {
            //redirect trace to console
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out));
            System.Diagnostics.Trace.AutoFlush = true;

            AzureProxyNode proxy = new AzureProxyNode();
            proxy.Init();
            while (true)
                Task.Delay(200).Wait();

        }
    }
}
