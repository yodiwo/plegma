using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ibm.iotf.client;
using com.ibm.iotf.client.device;

namespace Yodiwo.PaaS.IBM
{
    public class DevClient
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        private DeviceClient deviceClient;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public DevClient(string organization, string devtype, string devid, string authmethod, string authtoken)
        {
            // a device has already been registered in the IOTF
            deviceClient = new DeviceClient(organization, devtype, devid, authmethod, authtoken);
            //connect the device to the mqtt broker
            deviceClient.connect();
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void sendeventstoIOTF(string topic, string msg, byte QoS = 2)
        {
            if (deviceClient.isConnected())
            {
                //send commands to the IOTF
                // var pld = @"{ ""myValue"": ""99""}";
                deviceClient.publishEvent(topic, "json", msg, QoS);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AcceptCmdsFromtheIOTF(string cmd, byte QoS = 0)
        {
            //enable rx messages
            if (deviceClient.isConnected())
            {
                deviceClient.subscribeCommand(cmd, "json", QoS);
                deviceClient.commandCallback += DeviceClient_commandCallback;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void DeviceClient_commandCallback(string cmdName, string format, string data)
        {
            //handle commands from the IOTF
            DebugEx.TraceLog("Device Client Get cmd: " + cmdName + ", data:" + data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
