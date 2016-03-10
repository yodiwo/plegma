using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Yodiwo;
using Microsoft.Azure.Devices;

namespace Yodiwo.PaaS.Azure.Application
{
    public class AzureIOTApplication
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public EventHubClient eventHubClient;
        public ServiceClient serviceClient;
        private bool isRxEnabled = false;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnAzureIOTHubRx(string partition, string data);
        public OnAzureIOTHubRx OnAzureIOTHubRxCb = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        public AzureIOTApplication(string connectionString)
        {
            //create an eventhubclient for receiving messages from the Azure IOT Hub
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, "messages/events");
            //create a sericeclient, able to send messages to the Azure IOT Hub Devices
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        }
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void StartReceivingD2CMessages()
        {
            //update Rx flag
            lock (this)
                isRxEnabled = true;
            //find all partition ids
            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            //start Listening in all partitions
            foreach (string partition in d2cPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void StopReceiving()
        {
            //update rx flag
            lock (this)
                isRxEnabled = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            //create direct event consumer
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
            while (isRxEnabled)
            {
                //wait for receiving data
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                DebugEx.TraceLog("Application Message received. Partition: " + partition + " ,Data:" + data);
                if (OnAzureIOTHubRxCb != null)
                    OnAzureIOTHubRxCb(partition, data);

            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void ReceiveAckFromDev()
        {
            //enable ack rx when sending a message to an iot device
            ReceiveFeedbackAsync();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async void ReceiveFeedbackAsync()
        {
            //create feedback receiver
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();
            while (true)
            {
                //await for ack
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;
                DebugEx.TraceLog("Application Received feedback:" + string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void SendC2DMessages(string devname, string msg)
        {
            //send Message to the IOT device
            var commandMessage = new Message(Encoding.ASCII.GetBytes(msg));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            SendC2DMessagesAsync(devname, commandMessage);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async void SendC2DMessagesAsync(string devName, Message msg)
        {
            //send async messages
            await serviceClient.SendAsync(devName, msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
