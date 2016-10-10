using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yodiwo
{
    /// <summary>
    /// You can use this to enqueue request that will be consumed in the heartbeat thread in the proper order
    /// All methods of this class are thread-safe.
    /// </summary>
    public class RequestQueueConsumer<T>
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        Queue<T> RequestQueue = new Queue<T>();
        //------------------------------------------------------------------------------------------------------------------------
        bool _IsPaused = false;
        public bool IsPaused { get { return _IsPaused; } set { lock (RequestQueue) { _IsPaused = value; if (!value) Monitor.Pulse(RequestQueue); } } }
        //------------------------------------------------------------------------------------------------------------------------
#if UNIVERSAL
        Task heartbeat;
#else
        Thread heartbeat;
#endif
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void RequestHandlerDelegate(T item);
        RequestHandlerDelegate RequestHandler;
        //------------------------------------------------------------------------------------------------------------------------
        bool _IsAlive = false;
        public bool IsAlive { get { return _IsAlive; } }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> Time to sleep between each heartbeat </summary>
        public int SpinSleepPeriod = 0;
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> How many request are allowed to be queued before starting flooding counter-measures </summary>
        public int MaxQueuedRequests = int.MaxValue / 2;
        public enum FloodingProtectionMethods
        {
            BlockRequestThread,
            DropRequest,
        }
        public FloodingProtectionMethods FloodingProtection = FloodingProtectionMethods.BlockRequestThread;
        object floodLocker = new object();
        int floodLockerWaiting = 0;
        //------------------------------------------------------------------------------------------------------------------------
        bool _RetryLastItem = false;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public RequestQueueConsumer(RequestHandlerDelegate RequestHandler)
        {
            this.RequestHandler = RequestHandler;
            DebugEx.Assert(RequestHandler != null, "RequestHandler cannot be null");
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            lock (RequestQueue)
            {
                if (_IsAlive)
                    return;

                //set alive flag
                _IsAlive = true;

                //start heartbeat
#if UNIVERSAL
                heartbeat = Task.Factory.StartNew((Action)HeartBeatEntryPoint, TaskCreationOptions.LongRunning);
                heartbeat.Start();
#else
                heartbeat = new Thread(HeartBeatEntryPoint);
                heartbeat.Name = "RQC heartbeat";
                heartbeat.IsBackground = true;
                heartbeat.Start();
#endif
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void HeartBeatEntryPoint()
        {
            //spin
            while (_IsAlive)
            {
                T req;

                //pulse any waiting floodlocker
                lock (floodLocker)
                    Monitor.Pulse(floodLocker);

                //check pending requests.. if none the sleep the good sleep
                lock (RequestQueue)
                {
                    //wait for pulse in nothign in queue
                    if (RequestQueue.Count == 0 || _IsPaused)
                        Monitor.Wait(RequestQueue);

                    //double check (in case we woken up without items in queue)
                    if (RequestQueue.Count == 0 || _IsPaused)
                    {
                        continue;
                    }

                    //dequeue
                    req = RequestQueue.Peek();

                    //sleepy time
                    if (SpinSleepPeriod > 0)
                        Thread.Sleep(SpinSleepPeriod);
                }

                //handle it
                try
                {
                    _RetryLastItem = false;
                    //execute callback
                    RequestHandler(req);
                    //execute callback
                    if (!_RetryLastItem)
                    {
                        //consume it
                        lock (RequestQueue)
                            if (RequestQueue.Count > 0 && (req == null || req.Equals(RequestQueue.Peek())))
                                RequestQueue.Dequeue();
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "Unhandled exception caught. Thrown by RequestHandler()");
                    //consume it (eg. drop request)
                    lock (RequestQueue)
                        if (RequestQueue.Count > 0)
                            RequestQueue.Dequeue();
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void RequeueLastItem()
        {
            _RetryLastItem = true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>Must be called inside the RequestHandler</summary>
        public bool IsCurrentItemLatest()
        {
            lock (RequestQueue)
                return RequestQueue.Count == 1;
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> Avoid using this unless necessary. MaxQueuedRequests will not be honored when using this </summary>
        public void ForceRequeueSingleRequestOnTop(T Request) { ForceRequeueRequestOnTop(new[] { Request }); }
        /// <summary> Avoid using this unless necessary. MaxQueuedRequests will not be honored when using this </summary>
        public void ForceRequeueRequestOnTop(IEnumerable<T> Requests)
        {
            lock (RequestQueue)
            {
                var reqs = Requests.Concat(RequestQueue).ToArray();
                RequestQueue.Clear();
                foreach (var req in reqs)
                    RequestQueue.Enqueue(req);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Enqueue(T req)
        {
            lock (RequestQueue)
            {
                //flooding check
                if (RequestQueue.Count >= MaxQueuedRequests)
                {
                    if (FloodingProtection == FloodingProtectionMethods.DropRequest)
                        return;
                    else if (FloodingProtection == FloodingProtectionMethods.BlockRequestThread)
                    {
                        Interlocked.Increment(ref floodLockerWaiting);
                        //spin until thread can enqueue request
                        while (true)
                        {
                            lock (floodLocker)
                            {
                                Monitor.Exit(RequestQueue);
                                Monitor.Wait(floodLocker);
                            }
                            Monitor.Enter(RequestQueue);
                            if (RequestQueue.Count < MaxQueuedRequests)
                                break;
                        }
                        Interlocked.Decrement(ref floodLockerWaiting);
                    }
                    else
                    {
                        DebugEx.Assert("Unkown flooding protection");
                        return;
                    }
                }

                //enqueue
                RequestQueue.Enqueue(req);

                //pulse heartbeat
                Monitor.Pulse(RequestQueue);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Clear()
        {
            lock (RequestQueue)
                RequestQueue.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Pause() { IsPaused = true; }
        public void Continue() { IsPaused = false; }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            try
            {
#if UNIVERSAL
                Task _heartbeat = null;
#else
                Thread _heartbeat = null;
#endif
                lock (RequestQueue)
                {
                    if (!_IsAlive)
                        return;

                    _IsAlive = false;
                    RequestQueue.Clear();
                    Monitor.Pulse(RequestQueue);

                    //wait for heartbeat stop
                    _heartbeat = heartbeat;
                    heartbeat = null;
                }
                try
                {
                    //wait for finish
                    if (_heartbeat != null)
                    {
#if UNIVERSAL
                        _heartbeat.Wait(5000);
#else
                        _heartbeat.Join(5000);
#endif
                    }
                }
                catch { }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Unhandled exception in RQC Stop()"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~RequestQueueConsumer()
        {
            try
            {
                Stop();
            }
            catch
            { }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

    //==================================================================================================================

    /// <summary>
    /// You can use this to enqueue request that will be consumed in the heartbeat thread in the proper order
    /// All methods of this class are thread-safe.
    /// </summary>
    public class RequestQueueConsumer : RequestQueueConsumer<object>
    {
        public RequestQueueConsumer(RequestHandlerDelegate RequestHandler) : base(RequestHandler) { }
    }
}
