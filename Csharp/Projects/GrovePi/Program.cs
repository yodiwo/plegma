using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Yodiwo.Projects.GrovePi
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out));
            System.Diagnostics.Trace.AutoFlush = true;
            Transport trans = new Transport();
            //@"C:\Users\sodi\YodiwoDev\sw\projects\grovepi"
            trans.Init(@"C:\Python27\python.exe", "/home/yodiwo/YodiwoDev/test.py");
            // Accelerometer acceler = new Accelerometer(Pin.AnalogPin1, trans);
            //acceler.GetFirmwareVersion();
            //Task.Delay(3000).Wait();
            //acceler.Read();*/
            GrovePiNode grovepinode = new GrovePiNode(trans);
            grovepinode.Start();
            while (true)
                Task.Delay(500).Wait();


        }


    }
}
