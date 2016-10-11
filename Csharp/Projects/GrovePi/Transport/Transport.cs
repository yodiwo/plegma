using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Yodiwo;
using Newtonsoft.Json;
using System.Threading;


namespace Yodiwo.Projects.GrovePi
{
    public class Transport
    {
        // handlers standard input output
        StreamWriter sw;
        StreamReader sr;
        //python readerer task
        Task PythonReader;
        //handle events from python
        public delegate void OnRxMsg(SharpPy message);
        public event OnRxMsg OnRxMsgcb = delegate { };

        public void Init(string processname, string processargs)
        {
            //python process
            Console.WriteLine("=============>");

            var startinfo = new System.Diagnostics.ProcessStartInfo("sudo", "/usr/bin/python " + processargs);
            startinfo.RedirectStandardError = true;
            startinfo.RedirectStandardOutput = true;
            startinfo.RedirectStandardInput = true;
            startinfo.UseShellExecute = false;
            Process p;
            p = Process.Start(startinfo);
            sw = p.StandardInput;
            sr = p.StandardOutput;

            //write password to python standard input
            Task.Delay(4000).Wait();
            try { sw.WriteLine("1"); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Start Python");
            }
            //start reading from python
            PythonReader = Task.Run((Action)HandlePythonEvents);

        }

        private void HandlePythonEvents()
        {
            string eventstring;
            //continuous reading
            while (true)
            {
                try
                {
                    eventstring = sr.ReadLine();
                    if (eventstring == null)
                    {
                        Task.Delay(300).Wait();
                        continue;
                    }
                    while ((eventstring = sr.ReadLine()) != null)
                    {
                        if (eventstring.Length > 0)
                        {
                            lock (eventstring)
                            {
                                //DebugEx.TraceLog("Got from python: " + eventstring);
                                //todo
                                var msg = eventstring.FromJSON<SharpPy>();
                                OnRxMsgcb(msg);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceLog(ex.Message);
                }
            }
        }


        //send mesg for processing to python
        public void Send2python(SharpPy data)
        {
            lock (this)
            {
                if (data.payload == null)
                    data.payload = String.Empty;
                var csharp2pythonmessage = JsonConvert.SerializeObject(data);
                string timestamp = DateTime.Now.ToString();
                //DebugEx.TraceLog("Send to python: " + csharp2pythonmessage);
                sw.WriteLine(csharp2pythonmessage);
                sw.WriteLine("\n");
            }
        }
    }
}
