using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.mNode.Plugins.CPP
{
    static class Native
    {

        //------------------------------------------------------------------------------------------------------------------------
        /*
         * NOTES : 
         * - For linux(mono) to find the .so file a dllmap has been added in config (see App.config)
         * - Marshialing on variables will auto convert data to-and-from the native world using [MarshialAs()] attribute in front of the variable
         */
        //------------------------------------------------------------------------------------------------------------------------
        // Linux Version
        [DllImport("libplugin.so", EntryPoint = "OnTransportConnected", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OnTransportConnected();

        [DllImport("libplugin.so", EntryPoint = "OnTransportDisconnected", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OnTransportDisconnected();

        [DllImport("libplugin.so", EntryPoint = "GetMessage", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetMessage();

        [DllImport("libplugin.so", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Initialize(string ThingKeyPrefix);

        [DllImport("libplugin.so", EntryPoint = "Deinitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Deinitialize();

        [DllImport("libplugin.so", EntryPoint = "GetThings", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetThings();

        [DllImport("libplugin.so", EntryPoint = "OnPortEvent", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OnPortEvent(string ThingKeyPrefix);
        //------------------------------------------------------------------------------------------------------------------------
        //TODO: Windows Version
        //[DllImport("native.dll", EntryPoint = "calculate")]
        //static extern int dll_calculate(int a, int b);
        //------------------------------------------------------------------------------------------------------------------------        
    }
}
