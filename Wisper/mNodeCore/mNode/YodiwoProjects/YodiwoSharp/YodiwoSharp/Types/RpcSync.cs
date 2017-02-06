using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yodiwo
{
    public class WrapperMsg
    {
        /// <summary> message flags (request/response, etc) </summary>
        public eMsgFlags Flags;

        /// <summary>
        /// for RPC blocking calls: synchronization ID
        /// Message ID or Request message, or number of previous message that this message is responding to
        /// </summary>
        public int SyncId;

        /// <summary>
        /// JSON Serialized payload
        /// </summary>
        public string Payload;

        /// <summary>
        /// Size of packed/serialized payload
        /// </summary>
        public int PayloadSize;

        /// <summary>
        /// (un)mark message as request / check if request
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsRequest
        {
            get { return Flags.HasFlag(eMsgFlags.Request); }
            set
            {
                if (value == true)
                    Flags |= eMsgFlags.Request;
                else
                    Flags &= ~eMsgFlags.Request;
            }
        }

        /// <summary>
        /// (un)mark message as response / check if response
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsResponse
        {
            get { return Flags.HasFlag(eMsgFlags.Response); }
            set
            {
                if (value == true)
                    Flags |= eMsgFlags.Response;
                else
                    Flags &= ~eMsgFlags.Response;
            }
        }

        /// <summary>wrapper message flags</summary>
        [Flags]
        public enum eMsgFlags : byte
        {
            /// <summary>no flags (async message)</summary>
            None = 0,
            /// <summary>message is a request</summary>
            Request = 1 << 0,
            /// <summary>message is a response to a request</summary>
            Response = 1 << 1,
        }
    }

    public class RpcSync<T> where T : WrapperMsg
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        private class RpcWaiter
        {
            public object Response;
        }
        private Dictionary<int, RpcWaiter> RpcPending = new Dictionary<int, RpcWaiter>();
        //------------------------------------------------------------------------------------------------------------------------
        public const int DefaultRPCTimeout = 10 * 1000; // in milliseconds
        //------------------------------------------------------------------------------------------------------------------------
        protected int RpcSyncId = 0;
        //------------------------------------------------------------------------------------------------------------------------
        protected Func<T, object, bool> PublishFunc;

        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public RpcSync(Func<T, object, bool> pub_func)
        {
            PublishFunc += pub_func;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        protected virtual int GetNewSyncId()
        {
            var num = Interlocked.Increment(ref RpcSyncId);
            return num != 0 ? num : Interlocked.Increment(ref RpcSyncId);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual Trsp SendRequest<Trsp>(T wrapper_msg, object request, TimeSpan? timeout = null)
        {
            //null check
            if (wrapper_msg == null || request == null)
            {
                DebugEx.Assert("Null detected");
                return default(Trsp);
            }

            try
            {
                //create synchronization id
                var syncId = GetNewSyncId();

                //create waiter
                var w = new RpcWaiter();
                lock (RpcPending)
                    RpcPending.Add(syncId, w);

                //create wrapper message
                try
                {
                    wrapper_msg.IsRequest = true;
                    wrapper_msg.SyncId = syncId;
                    wrapper_msg.Payload = request.ToJSON();
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "msg wrapper create failed");
                    return default(Trsp);
                }


                //Send request and wait for response
                bool failed = false;
                lock (w)
                {
                    DebugEx.TraceLog("MsgSync: sending " + request.GetType() + ", syncid: " + wrapper_msg.SyncId);
                    PublishFunc?.Invoke(wrapper_msg, request);
                    //wait for response
#if DEBUG
                    Monitor.Wait(w);
#else
                    if (timeout.HasValue)
                    {
                        if (timeout.Value == TimeSpan.Zero)
                            Monitor.Wait(w); //if explicitly zero timespan, wait forever
                        else
                            Monitor.Wait(w, timeout.Value);
                    }
                    else
                        //if no value given, wait for default msec
                        Monitor.Wait(w, DefaultRPCTimeout);
#endif
                    if (w.Response == null)
                        failed = true;
                }
                if (failed)
                    lock (RpcPending)
                        RpcPending.Remove(syncId); //remove if failed to receive within time limit

                //give response back
                return (w.Response != null) ? (Trsp)w.Response : default(Trsp);
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
                return default(Trsp);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public virtual bool SendResponse(T wrapper_msg, object response, int syncId)
        {
            if (wrapper_msg == null || response == null)
                return false;
            try
            {
                wrapper_msg.IsResponse = true;
                wrapper_msg.SyncId = syncId;
                wrapper_msg.Payload = response.ToJSON();

                return PublishFunc?.Invoke(wrapper_msg, response) == true;
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "Could not send MsgSync response message");
                return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool OnMessageReceived(T wrapper_msg, object payload)
        {
            int syncId = wrapper_msg.SyncId;

            DebugEx.TraceLog("MsgSync: got " + payload.GetType() + ", syncid: " + wrapper_msg.SyncId);

            //check if this is a response, in which case unblock pending request
            if (wrapper_msg.IsResponse)
            {
                DebugEx.Assert(syncId != 0, "response without syncId not allowed");
                if (syncId == 0)
                    return false;

                //find waiter
                RpcWaiter w = null;
                lock (RpcPending)
                    if (RpcPending.TryGetValue(syncId, out w))
                        RpcPending.Remove(syncId); //remove if found

                //set result and wake
                if (w != null)
                {
                    lock (w)
                    {
                        w.Response = payload;
                        Monitor.Pulse(w);
                        return true;
                    }
                }
                else
                    DebugEx.TraceError("Could not find MsgSync waiter from " + payload.GetType() + " with syncId=" + syncId);
            }
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
