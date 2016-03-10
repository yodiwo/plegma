using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Yodiwo.PaaS.Azure.Application
{
    public class EventProcessorApplication
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        EventProcessorHost eventProcessorHost;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public EventProcessorApplication(string eventhostprocessorname, string eventhubname, string eventhubconnectionstring, string storageconnection)
        {
            //eventprocessor host manages incoming messages to the event hub service
            eventProcessorHost = new EventProcessorHost(eventhostprocessorname, eventhubname, EventHubConsumerGroup.DefaultGroupName, eventhubconnectionstring, storageconnection);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void RegisterEventProcessor()
        {
            //register eventhostprocessror to incoming messages
            Task.Run(() => eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>().Wait());

        }
        //------------------------------------------------------------------------------------------------------------------------
        public void UnRegisterEventProcessor()
        {
            //unregister eventhostprocessror from incoming messages
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


    }
}
