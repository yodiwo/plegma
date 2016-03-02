using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Yodiwo;
using Newtonsoft.Json;
namespace Yodiwo.Projects.SkyWriter
{


    //polling mechansim
    public class SensorWatcher
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        SkyWriterSensor sensor;
        //transport id the c#2py
        Transport sharppyiface;
        protected int WatcherId;
        protected int SyncId;
        DictionaryTS<int, Waiter> PendingRequests = new DictionaryTS<int, Waiter>();
        static int idGen = 0;

        public bool Active = false;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public SensorWatcher(SkyWriterSensor sensor, Transport sharppy)
        {
            this.sensor = sensor;
            this.sharppyiface = sharppy;
            this.sharppyiface.OnRxMsgcb += OnRxPythonMsg;
            //each sensor has its own watcherid
            WatcherId = Interlocked.Increment(ref idGen);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void StartContinuousMonitoring<T>(T req)
        {
            //start getting events from python
            StartMonitorEvents(req);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnRxPythonMsg(SharpPy msg)
        {
#if DEBUG
            DebugEx.TraceLog("OnRxPython " + msg.payload + ", syncid: " + msg.syncid + ", watcherid " + msg.watcherid);
            if (PendingRequests.Count != 0)
            {
                DebugEx.TraceLogInlineBegin("PENDING syncids: ");
                foreach (var item in PendingRequests.Keys)
                    DebugEx.TraceLogInline(item.ToString());
                DebugEx.TraceLogInlineEnd("");
            }
            DebugEx.TraceLog("This watcherid:" + this.WatcherId);
#endif
            if (Active)
            {
                if (msg.watcherid == this.WatcherId)
                {
                    int syncId = msg.syncid;
                    if (syncId == 0)
                        return;
                    var payload = this.sensor.DeserializePayload(msg.payload);
                    this.sensor.OnGetValue(payload);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual int GetNewSyncId()
        {
            var num = Interlocked.Increment(ref SyncId);
            return num != 0 ? num : Interlocked.Increment(ref SyncId);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void StartMonitorEvents<Req>(Req data)
        {
            //each event (i.e gesture, motion) has its own sync id
            var syncid = GetNewSyncId();
            var w = new Waiter();
            //construct sharppy msg
            SharpPy msg = new SharpPy()
            {
                isRequest = true,
                watcherid = WatcherId,
                syncid = syncid,
                payload = (data as SharpPy).payload,
                operation = (data as SharpPy).operation,
            };
            lock (w)
            {
                this.sharppyiface.Send2python(msg);
            }
            //DebugEx.TraceLog("continue w:" + WatcherId + " sid:" + syncid);
        }
        //------------------------------------------------------------------------------------------------------------------------

        public void SendMessage(string data)
        {
            SharpPy msg = new SharpPy()
            {
                payload = data
            };
            this.sharppyiface.Send2python(msg);
        }
        #endregion
    }
}
