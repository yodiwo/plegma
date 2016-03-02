using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yodiwo.Projects.SkyWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out));
            System.Diagnostics.Trace.AutoFlush = true;
            Transport trans = new Transport();
            //@"C:\Users\sodi\YodiwoDev\sw\projects\grovepi"
            trans.Init(@"/usr/bin/python", "/home/pi/YodiwoDev/theremin.py");
            SkyWriterNode skywriternode = new SkyWriterNode(trans);
            skywriternode.Start();
            while (true)
                Task.Delay(500).Wait();
        }
    }
}
