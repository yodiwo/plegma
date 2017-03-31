using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.mNode.Plugins.Samples.PInvoke
{
    public class PluginMain : Plugin
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        /*
         * NOTES : 
         * - For linux(mono) to find the .so file a dllmap has been added in config (see App.config)
         * - Marshialing on variables will auto convert data to-and-from the native world using [MarshialAs()] attribute in front of the variable
         */
        //------------------------------------------------------------------------------------------------------------------------
        // Linux Version
        [DllImport("native.so", EntryPoint = "calculate", CallingConvention = CallingConvention.Cdecl)]
        static extern int so_calculate(int a, int b);

        [DllImport("native.so", EntryPoint = "dequeue_Command", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        static extern string dequeue_Command();

        [DllImport("native.so", EntryPoint = "initialize", CallingConvention = CallingConvention.Cdecl)]
        static extern int initialize();

        [DllImport("native.so", EntryPoint = "deinitialize", CallingConvention = CallingConvention.Cdecl)]
        static extern int deinitialize();
        //------------------------------------------------------------------------------------------------------------------------
        //TODO: Windows Version
        //[DllImport("native.dll", EntryPoint = "calculate")]
        //static extern int dll_calculate(int a, int b);
        //------------------------------------------------------------------------------------------------------------------------        
        Thread CommandRetrieverThread;
        bool isRunning;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;

            //run
            try
            {
                var result = so_calculate(4, 5);
                DebugEx.TraceLog("Calling .so native function 4+5=" + result);
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Could not call .so native fuction"); }

            //initialize native system (threads etc)
            try
            {
                if (initialize() != 0)
                    DebugEx.TraceLog("Could not initialize native system");
                else
                {
                    DebugEx.TraceLog("Initialized native system");

                    //start command retriver
                    isRunning = true;
                    CommandRetrieverThread = new Thread(commandRetrieverHeartbeatEntryPoint);
                    CommandRetrieverThread.IsBackground = true;
                    CommandRetrieverThread.Start();
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Could not call .so native fuction"); }

            //init plugin
            DebugEx.TraceLog("Sample Plugin up and running !! ");

            //done
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Deinitialize()
        {
            try
            {
                //deinint native
                var nativeRet = deinitialize() == 0;

                //stop thread
                isRunning = false;
                CommandRetrieverThread?.Join(1000);
                CommandRetrieverThread = null;

                return nativeRet;
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "DeInitialize failed");
                return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void commandRetrieverHeartbeatEntryPoint()
        {
            while (isRunning)
            {
                try
                {
                    var cmd = dequeue_Command();
                    if (cmd != null)
                    {
                        try { processCommand(cmd); }
                        catch (Exception ex) { DebugEx.TraceError(ex, "processCommand failed"); }
                    }
                    else
                        Thread.Sleep(100); //something went wrong.. sleep for a while to avoid catastrophic spinning
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex, "Could not call .so native fuction");
                    return;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> Process commands received by the native library </summary>
        void processCommand(string cmd)
        {
            try
            {
                DebugEx.TraceLog("Got command from native library. command = " + cmd);
                //...
                //...
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "processCommand failed"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}

