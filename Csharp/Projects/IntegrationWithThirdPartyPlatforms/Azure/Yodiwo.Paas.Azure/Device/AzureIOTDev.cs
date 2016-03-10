using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Yodiwo.PaaS.Azure.Device
{
    public class AzureIOTDev
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        DeviceClient deviceClient;
        bool IsRxEnabled;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public AzureIOTDev(string iotHubUri, string devName, string devKey)
        {
            //create deviceCliebt using iothub hostname, device name and device key (aquired during registration)
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(devName, devKey));
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void SendD2CMessages(string messageString)
        {
            //send dev2cloud messages
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            SendD2CMessagesAsync(message);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async void SendD2CMessagesAsync(Message msg)
        {
            //async tx
            await deviceClient.SendEventAsync(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void StartReceiveC2DMessages()
        {
            //enable receiving C2D messages
            IsRxEnabled = true;
            ReceiveC2DAsync();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async void ReceiveC2DAsync()
        {
            while (IsRxEnabled)
            {
                //async rx
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;
                DebugEx.TraceLog("Received message: " + Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
