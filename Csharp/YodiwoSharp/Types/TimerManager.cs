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
    public class TimerManager<T>
    {
        public static Yodiwo.TimerManager<T> TimerMgr = new TimerManager<T>();

        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public class RequestInfo : PriorityQueueNode
        {
            public Int64 RequestID;
            public Action<T> Callback;
            public T UserData;
            public TimeSpan Interval;
            public int Count;
            public bool IsInfinitelyRepeatable;
            public bool IsCanceled = false;
        }
        Dictionary<Int64, RequestInfo> RequestLookup = new Dictionary<Int64, RequestInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        FutureCallbackQueue<RequestInfo> SleepRequests = new FutureCallbackQueue<RequestInfo>(QueueSize: 100 * 1000);
        //------------------------------------------------------------------------------------------------------------------------
        Int64 _idGen = 0;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public TimerManager()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        void callbackLanding(RequestInfo req)
        {
            // check if the request is canceled
            if (req.IsCanceled)
                return;

            // check if the Event is infinitely repeatable
            if (req.IsInfinitelyRepeatable)
                SleepRequests.Enqueue(callbackLanding, req, req.Interval);

            else
            {
                // resubmit timer if count > 0 
                req.Count--;
                if (req.Count > 0)
                    SleepRequests.Enqueue(callbackLanding, req, req.Interval);
            }

            //tick handler
            Task.Run(() => req.Callback(req.UserData));
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds an callback to be called every Interval as times as defined by Count parameter ( if not specified, the callback will be called infinitely)
        /// </summary>
        /// <param name="Callback"></param>
        /// <param name="UserData"></param>
        /// <param name="Interval"></param>
        /// <param name="Count"></param>
        /// <returns>A RequestID (used for ending the timer)</returns>
        public Int64 StartTimer(Action<T> Callback, T UserData, TimeSpan Interval, int Count = -1)
        {
            //declares
            Int64 id = 0;

            //create request
            var req = new RequestInfo()
            {
                Callback = Callback,
                UserData = UserData,
                Interval = Interval,
                IsInfinitelyRepeatable = Count == -1 ? true : false,
                Count = Count,
            };

            lock (this)
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
            }

            //start timer
            SleepRequests.Enqueue(callbackLanding, req, Interval);

            //return assigned request id
            return id;
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Tries to find RequestID in RequestLookup and removes it. If found returns true, elsewhere returns false.
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public bool EndTimer(Int64 RequestID)
        {
            lock (this)
            {
                // try to find RequestInfo
                RequestInfo req;
                if (RequestLookup.TryGetValue(RequestID, out req))
                {
                    // enable IsCanceled flag to this request 
                    req.IsCanceled = false;
                    RequestLookup.Remove(RequestID);
                    return true;
                }
                else
                    return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        public void Stop()
        {
            if (SleepRequests != null)
                SleepRequests.Stop();
        }
    }
}
