using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Yodiwo.Mqtt;
using Yodiwo.API.Plegma;
using uPLibrary.Networking.M2MqttClient.Messages;

namespace Yodiwo.NodeLibrary.Transports
{
    public class MQTT : Yodiwo.NodeLibrary.Transports.ITransportMQTT
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public Node Node;
        //------------------------------------------------------------------------------------------------------------------------
        private YMqttClient MqttClient;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsConnected { get { return MqttClient == null ? false : MqttClient.IsConnected; } }
        //------------------------------------------------------------------------------------------------------------------------
        public class RpcBlocker
        {
            public object Response;
        }
        private Dictionary<int, RpcBlocker> RpcPending = new Dictionary<int, RpcBlocker>();
        //------------------------------------------------------------------------------------------------------------------------
        string mqttCloudBrokerPubTopicPrefix;
        //------------------------------------------------------------------------------------------------------------------------
        private int RpcSyncId = 0;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------        
        public MQTT(Node Node)
        {
            this.Node = Node;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        private int GetNewSyncId()
        {
            var num = Interlocked.Increment(ref RpcSyncId);
            return num != 0 ? num : Interlocked.Increment(ref RpcSyncId);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public SimpleActionResult ConnectWithWorker(string mqttbroker, bool UseSsl)
        {
            //verify settings
            if (MqttClient != null && (MqttClient.BrokerHostname != mqttbroker))
            {
                MqttClient.Stop();
                MqttClient = null;
            }
            //create new client if needed
            if (MqttClient == null)
                MqttClient = new YMqttClient(mqttbroker, UseSsl);
            //start client
            if (MqttClient.IsConnected)
                return new SimpleActionResult() { IsSuccessful = true, Message = "MqttClient already connected" };
            else
            {
                var status = MqttClient.Start(Node.NodeKey,
                                              Node.NodeKey,
                                              Node.NodeSecret,
                                              cleanSession: false,
                                              rxEvent: MqttClientMsgPublishReceived,
                                              subscribedEvent: MqttClientMsgSubscribed,
                                              unsubscribedEvent: MqttClientMsgUnsubscribed);
                if (status == false)
                    return new SimpleActionResult() { IsSuccessful = false, Message = "MqttClient unable to Start" };
                else
                {
                    mqttCloudBrokerPubTopicPrefix = "/api/in/" + PlegmaAPI.APIVersion + "/" + Node.NodeKey.UserKey + "/" + Node.NodeKey + "/";
                    var topic = "/api/out/" + PlegmaAPI.APIVersion + "/" + Node.NodeKey + "/#";
                    MqttClient.SubscribeOne(topic, (byte)YMqttClient.eMqttQosLevels.ExactlyOnce);
                    return new SimpleActionResult() { IsSuccessful = true, Message = "Mqtt started successfuly" };
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Disconnect()
        {
            if (MqttClient != null)
            {
                MqttClient.Stop();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void MqttClientMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            DebugEx.TraceLog("MqttClient:: Message id " + e.MessageId + " subscribed successfully with QoS levels" + e.GrantedQoSLevels.ToStringEx());
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void MqttClientMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            DebugEx.TraceLog("MqttClient:: Message id " + e.MessageId + " unsubscribed successfully");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void MqttClientMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var mqttMsg = Encoding.UTF8.GetString(e.Message);

            DebugEx.TraceLog(System.DateTime.Now
                             + ": MqttClient: Received msg: " + mqttMsg
                             + " on topic: " + e.Topic
                             + ", retained: " + e.Retain
                             + ", QoS level: " + e.QosLevel
                             );


            MqttMsg wrapper_msg = null;
            try
            {
                wrapper_msg = mqttMsg.FromJSON<MqttMsg>();
                if (wrapper_msg != null)
                {
                    var apiMsgType = e.Topic.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    Object api_msg = null;

                    if (PlegmaAPI.ApiMsgNamesToTypes.ContainsKey(apiMsgType))
                        api_msg = wrapper_msg.Payload.FromJSON(PlegmaAPI.ApiMsgNamesToTypes[apiMsgType]);

                    //if valid API message
                    if (api_msg != null)
                    {
                        //check if it is a new Request
                        if (wrapper_msg.IsRequest)
                        {
                            var rsp = Node.HandleApiReq(api_msg);
                            if (rsp != null)
                            {
                                //send response if handler created one
                                SendResponse(rsp, wrapper_msg.SyncId);
                            }
                        }
                        //check if it is a response to a sent Request
                        else if (wrapper_msg.IsResponse)
                        {
                            //take through mqtt handler to handle synchronization
                            OnRxRsp(wrapper_msg, api_msg);
                        }
                        //otherwise, handle as a normal new Plegma API Message
                        else
                        {
                            Node.HandleApiMsg(api_msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Could not deserialize MqttAPIMessage");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void SendMessage(API.Plegma.ApiMsg message)
        {
            var msg = new MqttMsg()
            {
                Payload = message.ToJSON(HtmlEncode: false)
            };
            this.MqttClient.PublishMqttMsg(msg, this.mqttCloudBrokerPubTopicPrefix + PlegmaAPI.ApiMsgNames[message.GetType()]);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Trsp SendRequest<Trsp>(API.Plegma.ApiMsg request, TimeSpan? timeout = null)
        {
            //check
            if (request == null)
            {
                DebugEx.Assert("Request msg cannot be null");
                return default(Trsp);
            }

            //set default timeout
            if (timeout == null)
                timeout = TimeSpan.FromSeconds(15);

            //generate ID
            int syncId = GetNewSyncId();

            //create msg
            MqttMsg msg;
            try
            {
                msg = new MqttMsg()
                {
                    IsRequest = true,
                    SyncId = syncId,
                    Payload = request.ToJSON(HtmlEncode: false)
                };
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Could not create request msg");
                return default(Trsp);
            }

            //create waiter
            var w = new RpcBlocker();
            lock (RpcPending)
                RpcPending.Add(syncId, w);

            //wait for response
            lock (w)
            {
                //publish msg
                this.MqttClient.PublishMqttMsg(msg, this.mqttCloudBrokerPubTopicPrefix + PlegmaAPI.ApiMsgNames[request.GetType()]);
                if (timeout == null)
                    Monitor.Wait(w);
                else
                    Monitor.Wait(w, timeout.Value);
            }

            //remove if found
            lock (RpcPending)
                RpcPending.Remove(syncId);

            //give response back
            if (w.Response == null)
                return default(Trsp);
            else
                return (Trsp)w.Response;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void SendResponse(API.Plegma.ApiMsg response, int syncId)
        {
            var msg = new MqttMsg()
            {
                IsResponse = true,
                SyncId = syncId,
                Payload = response.ToJSON(HtmlEncode: false)
            };
            this.MqttClient.PublishMqttMsg(msg, this.mqttCloudBrokerPubTopicPrefix + PlegmaAPI.ApiMsgNames[response.GetType()]);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnRxRsp(MqttMsg wrapper_msg, object api_msg)
        {
            //sanity check
            DebugEx.Assert(wrapper_msg.SyncId != 0, "Responses must have syncId set to a non-zero number");
            if (wrapper_msg.SyncId == 0)
                return;

            //get id
            int syncId = wrapper_msg.SyncId;

            //find blocked transaction, if any
            RpcBlocker w = null;
            lock (RpcPending)
                if (RpcPending.TryGetValue(syncId, out w))
                    RpcPending.Remove(syncId); //remove if found

            //set result and wake
            if (w != null)
            {
                lock (w)
                {
                    w.Response = api_msg;
                    Monitor.Pulse(w);
                }
            }
            else
                DebugEx.TraceError("Could not find mqtt waiter from response message with ResponseToSeqNo=" + syncId);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
