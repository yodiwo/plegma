using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.GatewayMain
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out));
            System.Diagnostics.Trace.AutoFlush = true;
            GatewayNode gtwNode = new GatewayNode();
            gtwNode.Start();
            while (true)
            {
                Task.Delay(100000).Wait();
            }
        }
    }
}
