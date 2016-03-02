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


namespace Yodiwo.Projects.RasPiCamera
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
        public TCPImageServer tcpimageserver;
        Process p;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Init(string processname, string processargs)
        {
            tcpimageserver = new TCPImageServer();
            Task.Run(() =>
            {
                tcpimageserver.Start(8000);
            }
            );
            //wait for start
            while (tcpimageserver.Started == false)
                Task.Delay(200).Wait();
            DebugEx.TraceLog("TCP Server started");
            //python process
            p = new Process();
            p.StartInfo.FileName = processname;
            p.StartInfo.Arguments = processargs;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            Task.Delay(500).Wait();
            DebugEx.TraceLog("try to start python " + p.HasExited);
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
                    //DebugEx.TraceLog("Try to Read");
                    while ((eventstring = sr.ReadLine()) != null)
                    {
                        if (eventstring.Length > 0)
                        {
                            Console.WriteLine("Got from python: " + eventstring);
                            //todo
                            var msg = eventstring.FromJSON<SharpPy>();
                            OnRxMsgcb(msg);
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
                string timestamp = DateTime.Now.ToString();
                DebugEx.TraceLog("Send to python " + timestamp);
                sw.WriteLine(csharp2pythonmessage);
                sw.WriteLine("\n");
            }
        }
        #endregion
    }
}

