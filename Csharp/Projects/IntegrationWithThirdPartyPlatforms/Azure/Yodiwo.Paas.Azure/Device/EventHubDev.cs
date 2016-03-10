using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Yodiwo.PaaS.Azure.Device
{
    public class EventHubDev
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public string eventHubName;
        public string connectionName;
        public EventHubClient eventHubClient;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public EventHubDev(string evhubname, string connectionstring)
        {
            this.eventHubName = evhubname;
            this.connectionName = connectionstring;
            //create eventhub client
            eventHubClient = EventHubClient.CreateFromConnectionString(this.connectionName, this.eventHubName);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void PublishEvents(string msg)
        {
            //send messages to event hub client
            try
            {
                eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex.Message);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
