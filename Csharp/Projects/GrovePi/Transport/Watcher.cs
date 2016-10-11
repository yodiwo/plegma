using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Yodiwo;
using Newtonsoft.Json;
namespace Yodiwo.Projects.GrovePi
{


    //polling mechansim
    public class SensorWatcher
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        GrovePiSensor sensor;
        Task watchingTask;
        bool isRunning = false;
        //transport id the c#2py
        Transport sharppyiface;
        private string prevData = null;
        protected int WatcherId;
        protected int SyncId;
        DictionaryTS<int, Waiter> PendingRequests = new DictionaryTS<int, Waiter>();
        static int idGen = 0;
        //------------------------------------------------------------------------------------------------------------------------
        long _period;
        public long SamplingPeriod { get { return Interlocked.Read(ref _period); } set { Interlocked.Exchange(ref _period, value); } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public SensorWatcher(GrovePiSensor sensor, int period_msec, Transport sharppy)
        {
            this.sensor = sensor;
            this.sharppyiface = sharppy;
            this.sharppyiface.OnRxMsgcb += OnRxPythonMsg;
            WatcherId = Interlocked.Increment(ref idGen);
            SamplingPeriod = period_msec;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void StartContinuousMonitoring<T>(T req)
        {
            //start continuous reading
            isRunning = true;
            watchingTask = Task.Run(() => HeartBeatEntryPoint(req));
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void HeartBeatEntryPoint<T>(T req)
        {
            while (isRunning)
            {
                var rsp = GetSingleValue<T>(req);

                if (rsp != null)
                {
                    sensor.OnGetValue(rsp);
                }

                var wait_ms = Math.Max(200, (int)SamplingPeriod);
                Thread.Sleep(wait_ms);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void StopContinuousMonitoring()
        {
            //stop Monitoring
            isRunning = false;
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
            if (msg.watcherid == this.WatcherId)
            {
                int syncId = msg.syncid;
                if (syncId == 0)
                    return;
                Waiter w = null;
                w = PendingRequests.TryGetOrDefault(syncId);
                //set result and wake
                if (w != null)
                {
                    lock (w)
                    {
                        w.response = msg.payload;
                        Monitor.Pulse(w);
                    }
                }
                else
                    DebugEx.TraceLog("w is null");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual int GetNewSyncId()
        {
            var num = Interlocked.Increment(ref SyncId);
            return num != 0 ? num : Interlocked.Increment(ref SyncId);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public object GetSingleValue<Req>(Req data)
        {
            var syncid = GetNewSyncId();
            var w = new Waiter();
            lock (PendingRequests)
                PendingRequests.Add(syncid, w);
            //construct sharppy msg
            SharpPy msg = new SharpPy()
            {
                isRequest = true,
                watcherid = WatcherId,
                syncid = syncid,
                pin = this.sensor.Pin.ToString(),
                operation = (data as SharpPy).operation,
                payload = (data as SharpPy).payload,
            };
            lock (w)
            {
                this.sharppyiface.Send2python(msg);
                Monitor.Wait(w);
            }
            lock (PendingRequests)
                PendingRequests.Remove(syncid);

            if (w.response != null)
            {
                if (w.response != prevData && w.response != "255" && w.response != "65535")
                {
                    prevData = w.response;
                    DebugEx.TraceLog("CONT: wId:" + WatcherId + " sId:" + syncid + " response:" + w.response);
                    return this.sensor.DeserializePayload(w.response);
                }
                //else
                //    DebugEx.TraceLog("DROP: wId:" + WatcherId + " sId:" + syncid + " response:" + w.response);
            }
            return null;
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
