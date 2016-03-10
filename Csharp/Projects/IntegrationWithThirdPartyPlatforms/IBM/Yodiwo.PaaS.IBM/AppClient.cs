using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ibm.iotf.client;
using Yodiwo;

namespace Yodiwo.PaaS.IBM
{
    public class AppClient
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public ApplciationClient applicationClient;
        public delegate void OnThingEvent(string evtName, string message);
        public OnThingEvent OnThingEventCb = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        public AppClient(string organization, string appname, string apikey, string authtoken)
        {
            //create application endpoint
            applicationClient = new ApplciationClient(organization, appname, apikey, authtoken);
            //connect to mqtt broker
            applicationClient.connect();
            //register cb for devices' status
            applicationClient.deviceStatusCallback += StatusHandler;
            //register cb for the events that are published from devices to IOT Foundation
            applicationClient.eventCallback += EventHandler;
            //subscribe to both events
            //applicationClient.subscribeToDeviceStatus();
            applicationClient.subscribeToDeviceEvents();
        }
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        private void EventHandler(string evtName, string format, string data)
        {
            DebugEx.TraceLog("AppicationClient rx eventName: " + evtName + ", format: " + format + ", data:" + data);
            if (OnThingEventCb != null)
                OnThingEventCb(evtName, data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void StatusHandler(string deviceType, string deviceId, string data)
        {
            DebugEx.TraceLog("AppicationClient rx devicetype: " + deviceType + " ,deviceid: " + deviceId + ", data:" + data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void SendCommands(string devType, string devId, string command, string data)
        {
            applicationClient.publishCommand(devType, devId, command, "json", data);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
