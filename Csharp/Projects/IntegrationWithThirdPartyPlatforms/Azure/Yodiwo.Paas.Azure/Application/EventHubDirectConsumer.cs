using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Yodiwo.PaaS.Azure.Application
{
    public class EventHubDirectConsumer
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        EventHubReceiver eventHubDirectReceiver;
        bool isActive = false;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public EventHubDirectConsumer(EventHubClient client)
        {
            //create event hub direct consumer
            EventHubConsumerGroup group = client.GetDefaultConsumerGroup();
            var x = client.GetRuntimeInformation().PartitionIds[0];
            eventHubDirectReceiver = group.CreateReceiver(client.GetRuntimeInformation().PartitionIds[0]);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public async void StartConsumingMessages()
        {
            lock (this)
                isActive = true;
            while (isActive)
            {
                try
                {
                    var message = await eventHubDirectReceiver.ReceiveAsync();
                    if (message != null)
                    {
                        string msg;
                        var info = message.GetBytes();
                        msg = Encoding.UTF8.GetString(info);
                        DebugEx.TraceLog("Processing: Seq number " + message.SequenceNumber + " Offset " + message.Offset + "Partition " + message.PartitionKey + " EnqueueTimeUtc " + message.EnqueuedTimeUtc.ToShortTimeString() + " Message " + msg);

                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceLog("Exception on Receive" + ex.Message);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void StopConsumingMessages()
        {
            lock (this)
                isActive = false;

        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
