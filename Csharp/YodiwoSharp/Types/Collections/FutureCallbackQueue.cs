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
    public class FutureCallbackQueue<T>
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
        }
        HeapPriorityQueue<RequestInfo> RequestQueue = null;
        Dictionary<Int64, RequestInfo> RequestLookup = new Dictionary<Int64, RequestInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        Task heartbeat;
        object locker = new object();
        bool isRunning = true;
        //------------------------------------------------------------------------------------------------------------------------
        Int64 _idGen = 0;
        const int MaxSleepMiliseconds = 1 * 60 * 1000;
        const double MaxSleepMilisecondsD = (double)MaxSleepMiliseconds;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public FutureCallbackQueue(int QueueSize = 100 * 1000)
        {
            //create priority queue
            RequestQueue = new HeapPriorityQueue<RequestInfo>(QueueSize);
            //start heartbeat
            heartbeat = Task.Run((Action)HeartBeatEntryPoint);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        void HeartBeatEntryPoint()
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
                            //fire event
                            if (req.Callback != null)
                                req.Callback(req.UserData);
                            else if (req.WeakCallback != null)
                                req.WeakCallback.Invoke(req.UserData);
                            else
                                DebugEx.Assert("Should not be here");
                            //no timeout..keep spinning
                            timeout = null;
                        }
                    }

                    //sleep/wait manager
                    if (timeout.HasValue)
                    {
                        int _clampedTimeout = (int)timeout.Value.TotalMilliseconds.ClampCeil(MaxSleepMilisecondsD);
                        Monitor.Wait(locker, _clampedTimeout);
                    }

                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private long _Enqueue(Action<T> Callback, T UserData, TimeSpan SleepTime, DateTime wakeupTimestamp, bool StrongReference)
        {
            Int64 id = 0;

            //create request
            var req = new RequestInfo()
            {
                Callback = null,
                WeakCallback = null,
                UserData = UserData,
                SleepTime = SleepTime,
                WakeupTimestamp = wakeupTimestamp
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
        public Int64 Enqueue(Action<T> Callback, T UserData, TimeSpan SleepTime, bool StrongReference = false)
        {
            var wakeupTimestamp = DateTime.UtcNow + SleepTime;

            return _Enqueue(Callback, UserData, SleepTime, wakeupTimestamp, StrongReference);
        }

        //------------------------------------------------------------------------------------------------------------------------
        public Int64 Enqueue(Action<T> Callback, T UserData, DateTime dateTime, bool StrongReference = false)
        {
            var sleepTime = dateTime - DateTime.UtcNow;

            return _Enqueue(Callback, UserData, sleepTime, dateTime, StrongReference);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Stop()
        {
            lock (locker)
            {
                isRunning = false;
                Monitor.Pulse(locker);
                heartbeat.Wait(1000);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

    //==================================================================================================================

    /// <summary>
    /// You can use this to enqueue callbacks that will be called(async) at a specific future timestamp
    /// </summary>
    public class FutureCallbackQueue : FutureCallbackQueue<object>
    {
        public static readonly FutureCallbackQueue GlobalQueue = new FutureCallbackQueue();

        public FutureCallbackQueue(int QueueSize = 100 * 1000) : base(QueueSize: QueueSize) { }
    }
}
