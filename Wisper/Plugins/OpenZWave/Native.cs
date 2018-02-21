using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.mNode.Plugins.Bridge_OZWave
{
    class Native
    {
        /// <summary> Start manager </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "init", CallingConvention = CallingConvention.Cdecl)]
        public static extern int init(string savePath);

        /// <summary> stop manager </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "deInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int deInit(UInt32 homeID);

        /// <summary> Receive notifications from zwave network </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "GetMessage", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetMessage();

        /// <summary> connect to controller </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "Connect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Connect(string ControllerPort);

        /// <summary> Start add device operation </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "AddDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddDevice(bool isSecure, UInt32 homeID);

        /// <summary> Start remove device operation </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "RemoveDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RemoveDevice(UInt32 homeID);

        /// <summary> Cancel controller operation </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "CancelOperation", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CancelOperation(UInt32 homeID);

        /// <summary> Save paired zwave devices current configuration </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "saveZWaveDevices", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool saveZWaveDevices(UInt32 homeID);

        /// <summary> change device value request </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "ChangeDeviceValue", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ChangeDeviceValue(string msg, string newValue, UInt32 homeId);

        /// <summary> change device association groups </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "ChangeGroupAssociation", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ChangeGroupAssociation(UInt32 homeId, byte nodeId, byte targetNodeId, byte instance, byte group);

        /// <summary> change device association groups </summary>
        [DllImport("/home/pi/open-zwave1/libopenzwave.so", EntryPoint = "GetNodeState", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string GetNodeState(UInt32 homeId, byte nodeId);
    }
}