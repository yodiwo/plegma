using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Yodiwo.Projects.RasPiCamera
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out));
            System.Diagnostics.Trace.AutoFlush = true;
            Transport transport = new Transport();
            transport.Init("/usr/bin/python", "/home/pi/YodiwoDev/imageclient.py");
            RaspberryNode raspberrynode = new RaspberryNode(transport);
            raspberrynode.Start();
            while (true)
            {
                Task.Delay(500).Wait();
            }
        }
    }
}
