import json
import paho.mqtt.client as mqtt

import PyNodeHelper
from PyNodeHelper import Configuration
from plegma.PlegmaApi import PlegmaApi, MqttMsg, eMsgFlags

TAG = "mqttclient"
KEEPALIVESEC = 60
connack_codes = {"CONNACK_ACCEPTED": 0,
                 "CONNACK_REFUSED_PROTOCOL_VERSION": 1,
                 "CONNACK_REFUSED_IDENTIFIER_REJECTED": 2,
                 "CONNACK_REFUSED_SERVER_UNAVAILABLE": 3,
                 "CONNACK_REFUSED_BAD_USERNAME_PASSWORD": 4,
                 "CONNACK_REFUSED_NOT_AUTHORIZED": 5
                 }


class MqttClient(object):
    def __init__(self):
        self.config = Configuration()
        self.plegmaapi = PlegmaApi()
        self.syncId = 0
        self._on_PlegmaMsgArrived = None
        self._on_PlegmaReqArrived = None
        self._on_PlegmaRspArrived = None
        self.InitClient()

    # Helpers
    # ------------------------------------------------------------------------------------------------------------------
    def InitClient(self):
        PyNodeHelper.LOG.Info(TAG, "Init client")
        self.client = mqtt.Client(self.config.nodekey.toString())
        self.port = None
        if self.config.mqttbrokerusessl:
            self.port = 8883
            try:
                self.client.tls_set(ca_certs=self.config.mqttbrokercert)
            except Exception as e:
                PyNodeHelper.LOG.Error(TAG, "tsl_set, error: {}".format(str(e)))
        else:
            self.port = 1883
        self.is_connected = False
        self.client.on_connect = self.on_connect
        self.client.on_disconnect = self.on_disconnect
        self.client.on_message = self.on_message
        self.client.on_publish = self.on_publish
        self.client.on_subscribe = self.on_subscribe
        self.client.on_unsubscribe = self.on_unsubscribe

    def Subscribe(self, topic, qos):
        if self.mqttSubTopicPrefix not in topic:
            PyNodeHelper.LOG.Error(TAG, "Invalid topic prefix")
            raise ValueError("Invalid topic prefix")
        self.client.subscribe(topic, qos)

    def Unsubscribe(self, topic):
        if self.mqttSubTopicPrefix not in topic:
            PyNodeHelper.LOG.Error(TAG, "Invalid topic prefix")
            raise ValueError("Invalid topic prefix")
        self.client.unsubscribe(topic)

    def Send(self, topic, mqtt_msg, qos):
        PyNodeHelper.LOG.Info(TAG, "Publish --> topic: {0}, message: {1}, qos: {2}".format(topic, mqtt_msg, qos))
        self.Publish(topic, mqtt_msg, qos)

    def Publish(self, topic, payload, qos):
        if self.mqttPubTopicPrefix not in topic:
            PyNodeHelper.LOG.Error(TAG, "Invalid topic prefix")
            raise ValueError("Invalid topic prefix")
        self.client.publish(topic, payload, qos=qos)

    def MqttMsgConnack(self, rc):
        for key in connack_codes.keys():
            if connack_codes[key] == rc:
                return key

    # Public
    # ------------------------------------------------------------------------------------------------------------------
    def Connect(self):
        self.mqttPubTopicPrefix = "/api/in/1/{0}/{1}/".format(self.config.userkey, self.config.nodekey.__str__())
        self.mqttSubTopicPrefix = "/api/out/1/" + self.config.nodekey.__str__() + "/#"
        self.client.username_pw_set(self.config.nodekey.__str__(), self.config.nodesecretkey)
        self.client.connect(self.config.mqttbrokeraddress, self.port, KEEPALIVESEC)
        self.client.loop_start()

    def Disconnect(self):
        if self.is_connected:
            self.client.disconnect()
            self.client.loop_stop()

    def SendMsg(self, message, qos):
        self.syncId += 1
        topic = self.mqttPubTopicPrefix + self.plegmaapi.ApiMsgNames[type(message)]
        mqtt_msg = MqttMsg(SyncId=self.syncId, Payload=message.toJson(), Flags=eMsgFlags.Message)
        jsonmqttmsg = json.dumps(mqtt_msg.__dict__)
        print(jsonmqttmsg)
        self.Send(topic, jsonmqttmsg, qos)

    def SendReq(self, message, qos):
        self.syncId += 1
        topic = self.mqttPubTopicPrefix + self.plegmaapi.ApiMsgNames[type(message)]
        mqtt_msg = MqttMsg(SyncId=self.syncId, Payload=message.toJson(), Flags=eMsgFlags.Request)
        print(mqtt_msg)
        jsonmqttmsg = json.dumps(mqtt_msg.__dict__)
        print(jsonmqttmsg)
        self.Send(topic, jsonmqttmsg, qos)

    def SendRsp(self, message, syncId, qos):
        topic = self.mqttPubTopicPrefix + self.plegmaapi.ApiMsgNames[type(message)]
        mqtt_msg = MqttMsg(SyncId=syncId, Payload=message.toJson(), Flags=eMsgFlags.Response)
        jsonmqttmsg = json.dumps(mqtt_msg.__dict__)
        print(jsonmqttmsg)
        self.Send(topic, jsonmqttmsg, qos)

    # Custom Cbs
    # ------------------------------------------------------------------------------------------------------------------
    def on_PlegmaMsgArrived(self, func):
        self._on_PlegmaMsgArrived = func

    def on_PlegmaRspArrived(self, func):
        self._on_PlegmaRspArrived = func

    def on_PlegmaReqArrived(self, func):
        self._on_PlegmaReqArrived = func

    # Mqtt library Cbs
    # ------------------------------------------------------------------------------------------------------------------
    def on_message(self, client, userdata, msg):
        PyNodeHelper.LOG.Info(TAG, "On message arrived --> Topic: {0}, qos: {1}, payload: {2}".format(msg.topic, str(msg.qos), str(msg.payload)))
        parsedmqttmsg = json.loads(str(msg.payload, "utf-8"))
        payload = parsedmqttmsg["Payload"]
        apimsgtype = msg.topic.split("/")[-1]
        objmsg = self.plegmaapi.ApiMsgNamesToTypes[apimsgtype].fromCloud(json.loads(payload))
        wrapper_msg = MqttMsg.fromCloud(parsedmqttmsg)
        syncid = int(parsedmqttmsg["SyncId"])
        if wrapper_msg.isRequest():
            return self._on_PlegmaReqArrived(objmsg, syncid)
        elif wrapper_msg.isResponse():
            return self._on_PlegmaRspArrived(objmsg, syncid)
        else:
            return self._on_PlegmaMsgArrived(objmsg, syncid)

    def on_connect(self, client, userdata, flags, rc):
        PyNodeHelper.LOG.Info(TAG, "Connected with result code {0}".format(str(rc)))
        PyNodeHelper.LOG.Info(TAG, "Connected with state ".format(str(self.client._state)))
        PyNodeHelper.LOG.Info(TAG, "Connected with connack message ".format(self.MqttMsgConnack(rc)))
        self.is_connected = True
        self.Subscribe(self.mqttSubTopicPrefix, 2)

    def on_disconnect(self, mqttc, userdata, rc):
        self.is_connected = False
        if rc != 0:
            PyNodeHelper.LOG.Info(TAG, "Unexpected disconnection. Reconnecting...")
            mqttc.reconnect()
        else:
            PyNodeHelper.LOG.Info(TAG, "Disconnected successfully")

    def on_publish(self, client, userdata, mid):
        PyNodeHelper.LOG.Info(TAG, "Publish message: " + "mid: {}".format(str(mid)))

    def on_subscribe(self, client, userdata, mid, granted_qos):
        PyNodeHelper.LOG.Info(TAG, "Subscribed: {0} {1}".format(str(mid), str(granted_qos)))

    def on_unsubscribe(self, client, userdata, mid, granted_qos):
        PyNodeHelper.LOG.Info(TAG, "Unsubscribed: {0} {1}".format(str(mid), str(granted_qos)))
