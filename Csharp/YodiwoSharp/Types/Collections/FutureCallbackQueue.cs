using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yodiwo
{
    /// <summary>
    /// You can use this to enqueue callbacks that will be called(async) at a specific future timestamp
    /// </summary>
    public class FutureCallbackQueue<T> : IDisposable
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        class RequestInfo : PriorityQueueNode
        {
            public Int64 RequestID;
            public Action<T> Callback;
            public WeakAction<T> WeakCallback;
            public T UserData;
            public TimeSpan SleepTime;
            public DateTime WakeupTimestamp;
            public bool AsyncCallback;
        }
        HeapPriorityQueue<RequestInfo> RequestQueue = null;
        Dictionary<Int64, RequestInfo> RequestLookup = new Dictionary<Int64, RequestInfo>();
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        Thread heartbeat;
#elif UNIVERSAL
        Task heartbeat;
#endif
        object locker = new object();
        bool isRunning = true;
        //------------------------------------------------------------------------------------------------------------------------
        Int64 _idGen = 0;
        const int MaxSleepMiliseconds = 1 * 60 * 1000;
        const double MaxSleepMilisecondsD = (double)MaxSleepMiliseconds;
        //------------------------------------------------------------------------------------------------------------------------
        bool _IsDisposed = false;
        public bool IsDisposed => _IsDisposed;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public FutureCallbackQueue(int QueueSize = 100 * 1000)
        {
            //create priority queue
            RequestQueue = new HeapPriorityQueue<RequestInfo>(QueueSize);
            //start heartbeat
#if NETFX
            heartbeat = new Thread(HeartBeatEntryPoint);
            heartbeat.Name = "FutureCallbackQueue heartbeat";
            heartbeat.IsBackground = true;
            heartbeat.Start();
#elif UNIVERSAL
            heartbeat = Task.Factory.StartNew((Action)HeartBeatEntryPoint, TaskCreationOptions.LongRunning);
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        void HeartBeatEntryPoint()
        {
            try
            {
                //declares
                TimeSpan? timeout = null;
                //spin
                while (isRunning)
                {
                    lock (locker)
                    {
                        //check pending requests.. if none the sleep the good sleep
                        if (RequestQueue.Count == 0)
                            timeout = TimeSpan.MaxValue; //infinite
                        else
                        {
                            //get first pending request (has smallest wake timestamp)
                            var req = RequestQueue.First();
                            //get remaining time
                            var rem = req.WakeupTimestamp - DateTime.UtcNow;
                            if (rem.TotalMilliseconds > 0.5d)
                                timeout = rem;
                            else
                            {
                                //consume event
                                RequestQueue.Dequeue();
                                RequestLookup.Remove(req.RequestID);
                                //callback
                                if (req.AsyncCallback)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            //fire event
                                            if (req.Callback != null)
                                                req.Callback(req.UserData);
                                            else if (req.WeakCallback != null)
                                                req.WeakCallback.Invoke(req.UserData);
                                            else
                                                DebugEx.Assert("FutureCallbackQueue reqId:" + req.RequestID + " has no callback to invoke");
                                        }
                                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception in callback caught by FutureCallbackQueue"); }
                                    });
                                }
                                else
                                {
                                    //exit lock to fire event
                                    Monitor.Exit(locker);
                                    try
                                    {
                                        //fire event
                                        if (req.Callback != null)
                                            req.Callback(req.UserData);
                                        else if (req.WeakCallback != null)
                                            req.WeakCallback.Invoke(req.UserData);
                                        else
                                            DebugEx.Assert("FutureCallbackQueue reqId:" + req.RequestID + " has no callback to invoke");
                                    }
                                    catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception in callback caught by FutureCallbackQueue"); }
                                    finally { Monitor.Enter(locker); }
                                }
                                //no timeout..keep spinning
                                timeout = null;
                            }
                        }

                        //sleep/wait manager
                        if (timeout.HasValue)
                        {
                            int _clampedTimeout = (int)timeout.Value.TotalMilliseconds.ClampCeil(MaxSleepMilisecondsD);
                            Monitor.Wait(locker, _clampedTimeout);
                            if (!isRunning)
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "FutureCallbackQueue heartbeat failed"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private long _Enqueue(Action<T> Callback, T UserData, TimeSpan SleepTime, DateTime wakeupTimestamp, bool StrongReference, bool AsyncCallback)
        {
            Int64 id = 0;

            //create request
            var req = new RequestInfo()
            {
                Callback = null,
                WeakCallback = null,
                UserData = UserData,
                SleepTime = SleepTime,
                WakeupTimestamp = wakeupTimestamp,
                AsyncCallback = AsyncCallback,
            };

            //setup callbacks
            if (StrongReference)
                req.WeakCallback = WeakAction<T>.Create(Callback);
            else
                req.Callback = Callback;

            //enqueue
            lock (locker)
            {
                //generate id
                do
                {
                    _idGen++;
                }
                while (RequestLookup.ContainsKey(_idGen));

                //keep id
                id = _idGen;
                req.RequestID = id;

                //add to sleeprequests
                RequestLookup.Add(id, req);
                RequestQueue.Enqueue(req, wakeupTimestamp.Ticks);

                //pulse heartbeat
                Monitor.Pulse(locker);
            }

            //return assigned request id
            return id;
        }

        //------------------------------------------------------------------------------------------------------------------------
        public Int64 Enqueue(Action<T> Callback, T UserData, TimeSpan SleepTime, bool StrongReference = false, bool AsyncCallback = true)
        {
            var wakeupTimestamp = DateTime.UtcNow + SleepTime;

            return _Enqueue(Callback, UserData, SleepTime, wakeupTimestamp, StrongReference, AsyncCallback);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Int64 Enqueue(Action<T> Callback, T UserData, DateTime dateTime, bool StrongReference = false, bool AsyncCallback = true)
        {
            var sleepTime = dateTime - DateTime.UtcNow;

            return _Enqueue(Callback, UserData, sleepTime, dateTime, StrongReference, AsyncCallback);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Cancel(Int64 id)
        {
            lock (locker)
            {
                var req = RequestLookup.TryGetOrDefault(id);
                RequestLookup.Remove(id);
                if (req != null)
                {
                    RequestQueue.Remove(req);
                    Monitor.Pulse(locker);
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            lock (locker)
            {
                isRunning = false;
                Monitor.Pulse(locker);
#if NETFX
                heartbeat?.Join(1000);
#elif UNIVERSAL
                heartbeat?.Wait(1000);
#endif
                heartbeat = null;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            if (_IsDisposed)
                return;

            //dispose 
            try
            {
                lock (locker)
                {
                    if (_IsDisposed)
                        return;
                    else
                        _IsDisposed = true;

                    //stop it
                    if (isRunning)
                        Stop();
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "FutureCallbackQueue dispose failed"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~FutureCallbackQueue()
        {
            try
            {
                if (!_IsDisposed)
                    Dispose();
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "FutureCallbackQueue destructor failed"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

    //==================================================================================================================

    /// <summary>
    /// You can use this to enqueue callbacks that will be called(async) at a specific future timestamp
    /// </summary>
    public class FutureCallbackQueue : FutureCallbackQueue<object>
    {
        public FutureCallbackQueue(int QueueSize = 100 * 1000) : base(QueueSize: QueueSize) { }
    }
}
