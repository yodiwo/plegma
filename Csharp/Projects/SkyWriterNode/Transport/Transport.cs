using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Yodiwo;
using Newtonsoft.Json;

namespace Yodiwo.Projects.SkyWriter
{
    public class Transport
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        // handlers standard input output
        StreamWriter sw;
        StreamReader sr;
        //python readerer task
        Task PythonReader;
        //handle events from python
        public delegate void OnRxMsg(SharpPy message);
        public event OnRxMsg OnRxMsgcb = delegate { };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Init(string processname, string processargs)
        {
            //python process
            Process p = new Process();
            p.StartInfo.FileName = processname;
            p.StartInfo.Arguments = processargs;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            //redirect standard input abd outpout to steamers
            sw = p.StandardInput;
            sr = p.StandardOutput;
            //start reading from python
            PythonReader = Task.Run((Action)HandlePythonEvents);

            //python process start
        }

        //------------------------------------------------------------------------------------------------------------------------
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
                        //Console.Write(".");
                        if (eventstring.Length > 0)
                        {
                            if (eventstring.ElementAt(0) != '{')
                            {
                                //why do i receive non-json?
                                Console.WriteLine("non-json???");
                                //Task.Delay(300).Wait();
                                //eventstring = sr.ReadLine();
                                continue;
                            }
                            //DebugEx.TraceLog("Got from python: " + eventstring);
                            //todo
                            var msg = eventstring.FromJSON<SharpPy>();
                            OnRxMsgcb(msg);
                        }
                        else
                        {
                            Console.WriteLine("eventstring length equal to 0");
                            //Task.Delay(300).Wait();
                            //eventstring = sr.ReadLine();
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceLog(ex.Message);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        //send mesg for processing to python
        public void Send2python(SharpPy data)
        {
            lock (this)
            {
                if (data.payload == null)
                    data.payload = String.Empty;
                var csharp2pythonmessage = JsonConvert.SerializeObject(data);

                //DebugEx.TraceLog("Send to python " + csharp2pythonmessage);
                sw.WriteLine(csharp2pythonmessage);
                sw.WriteLine("\n");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        #endregion
    }
}

