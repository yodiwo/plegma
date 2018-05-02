import PyNodeHelper
from PyNodeHelper import Configuration,Converter
from MqttClient import MqttClient
from plegma.Messages import *
from plegma.Port import Port
from plegma.Thing import Thing

TAG = "NodeService"


class NodeService(object):
    ThingRevisionNumber = 0 # last synced revision number
    def __init__(self):
        self.activePortKeys = {}  # list activePortKeys
        self._Things = {}  # dict thingkey -> Thing
        self.config = Configuration()
        self.mqttclient = None

    # Start/Teardown
    #  ------------------------------------------------------------------------------------------------------------------
    def Start(self, protocol):
        if protocol == PyNodeHelper.eMessagingProtocol.Mqtt:
            PyNodeHelper.LOG.Info(TAG, "Starting . . . MQTT")
            self.mqttclient = MqttClient()
            self.mqttclient._on_PlegmaMsgArrived = self.on_PlegmaMsgArrived
            self.mqttclient._on_PlegmaRspArrived = self.on_PlegmaRspArrived
            self.mqttclient._on_PlegmaReqArrived = self.on_PlegmaReqArrived
            self.mqttclient.Connect()

    def Teardown(self):
        if self.mqttclient is not None:
            PyNodeHelper.LOG.Info(TAG, "Teardown . . . MQTT")
            self.mqttclient.Disconnect()

    # Import Things
    #  ------------------------------------------------------------------------------------------------------------------
    def ImportThings(self, things):
        self._Things = things

    # Mqtt client Cbs
    # ------------------------------------------------------------------------------------------------------------------
    def on_PlegmaMsgArrived(self, msg, syncId):
        PyNodeHelper.LOG.Info(TAG, "PlegmaMsgArrived . . . ")
        self.HandleApiMsg(msg)

    def on_PlegmaReqArrived(self, msg, syncId):
        PyNodeHelper.LOG.Info(TAG, "PlegmaReqArrived . . . ")
        rsp_msg = self.HandleApiReq(msg)
        if rsp_msg:
            self.SendRsp(rsp_msg, syncId)

    def on_PlegmaRspArrived(self, msg, syncId):
        PyNodeHelper.LOG.Info(TAG, "PlegmaRspArrived . . . ")
        self.HandleApiRsp(msg, syncId)

    # Plegma API Handlers
    #  ------------------------------------------------------------------------------------------------------------------
    def HandleApiReq(self, req):
        if isinstance(req, type(NodeInfoReq())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: NodeInfoReq")
            self.__class__.ThingRevisionNumber = req.ThingsRevNum
            return self.HandleNodeInfoReq()
        elif isinstance(req, type(ThingsGet())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: ThingsGet")
            return self.HandleThingsGet(req)
        elif isinstance(req, type(ThingsSet())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: ThingsSet")
            return self.HandleThingsSet(req)
        elif isinstance(req, type(PortStateGet())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: PortStateGet")
            return self.HandlePortStateGet(req)
        elif isinstance(req, type(PortStateSet())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: PortStateSet")
            return self.HandlePortStateSet(req)
        elif isinstance(req, type(NodeStatusChangedReq())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: NodeStatusChangedReq")
            return self.HandleNodeStatusChangedReq(req)
        else:
            PyNodeHelper.LOG.Error(TAG, "Req Type: UNKNOWN ")
            raise ValueError("Unexpected message")

    def HandleApiRsp(self, msg, syncId):
        # TODO:
        return

    def HandleApiMsg(self, msg):
        if isinstance(msg, type(ActivePortKeysMsg())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: ActivePortkeysMsg")
            self.HandleActivePortkeysMsg(msg)
        elif isinstance(msg, type(PortEventMsg())):
            PyNodeHelper.LOG.Info(TAG, "Req Type: PortEventMsg")
            self.HandlePortEventMsg(msg)
        else:
            PyNodeHelper.LOG.Error(TAG, "Unexpected message")
            raise ValueError("Unexpected message")

    def HandleNodeInfoReq(self):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        self.UpdateRevNum();
        return NodeInfoRsp(Name=self.config.nodename, SeqNo=0, ThingsRevNum=self.__class__.ThingRevisionNumber, Type=eNodeType.Generic,
                           Capabilities=eNodeCapa.nodeCapaNone)

    def HandlePortStateGet(self, msg):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        portstates = []
        if msg.Operation == ePortStateOperation.AllPortStates:
            for thing in self._Things.values():
                for port in thing.Ports:
                    isdep = True if port.PortKey in self.activePortKeys else False
                    portstates.append(PortState(PortKey=port.PortKey, State=port.State, IsDeployed=isdep, RevNum=self.__class__.ThingRevisionNumber))
        elif msg.Operation == ePortStateOperation.ActivePortStates:
            for pkey in self.activePortKeys:
                thingkey = Converter.PortkeyToThingkey(pkey)
                thing = self._Things[thingkey]
                for port in thing.Ports:
                    if pkey == port.PortKey:
                        portstates.append(PortState(PortKey=pkey, State=port.State, IsDeployed=True, RevNum=self.__class__.ThingRevisionNumber))
        elif msg.Operation == ePortStateOperation.SpecificKeys:
            if msg.PortKeys:
                for pkey in msg.PortKeys:
                    thingkey = Converter.PortkeyToThingkey(pkey)
                    thing = self._Things[thingkey]
                    for port in thing.ports:
                        if port.PortKey == pkey:
                            isdep = True if pkey in self.activePortKeys else False
                            portstates.append(PortState(PortKey=pkey, State=port.State, IsDeployed=isdep, RevNum=self.__class__.ThingRevisionNumber))

        PyNodeHelper.LOG.Info(TAG, "Handling finished")
        return PortStateSet(Operation=msg.Operation, PortStates=portstates)

    def HandlePortStateSet(self, msg):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        for portstate in msg.PortStates:
            portstate = PortState.fromCloud(portstate)
            if (portstate is not None):
                pk = portstate.PortKey
                thingKey = Converter.PortkeyToThingkey(pk)
                thing = self._Things[thingKey]
                for p in thing.Ports:
                    if p.PortKey == pk:
                        p.State = portstate.State
                self._Things[thingKey] = thing
            else:
                PyNodeHelper.LOG.Error(TAG, "Invalid PortState [missing attributes]. Ignoring...")

        PyNodeHelper.LOG.Info(TAG, "Handling finished")
        return GenericRsp(IsSuccess=True)

    def HandleNodeStatusChangedReq(self, msg):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        #if msg.NewStatus == eNodeNewStatus.Unpaired:
       # TODO: UNPAIR!!!

        PyNodeHelper.LOG.Info(TAG, "Handling finished")
        return GenericRsp(IsSuccess=True)

    def HandlePortEventMsg(self, msg):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        for portevent in msg.PortEvents:
            portevent = PortEvent.fromCloud(portevent)
            if (portevent is not None):
                pk = portevent.PortKey
                thingKey = Converter.PortkeyToThingkey(pk)
                thing = self._Things[thingKey]
                for p in thing.Ports:
                    if p.PortKey == pk:
                        p.State = portevent.State
            else:
                PyNodeHelper.LOG.Error(TAG,"Invalid PortEvent [missing attributes]. Ignoring...")

        PyNodeHelper.LOG.Info(TAG, "Handling finished")

    def HandleThingsGet(self, req):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        op = req.Operation
        revNum = req.RevNum
        rsp_msg = ThingsSet(Operation=eThingsOperation.Invalid, RevNum=self.__class__.ThingRevisionNumber)
        if revNum >= rsp_msg.RevNum:
            return None
        if len(self._Things) == 0:
            return None
        if op == eThingsOperation.Get:
            rsp_msg.Operation = eThingsOperation.Overwrite
            rsp_msg.Status = True
            rsp_msg.Data = [thing for thing in self._Things.values()]
        elif op == eThingsOperation.Scan:
            pass

        PyNodeHelper.LOG.Info(TAG, "Handling finished")
        return rsp_msg

    def HandleThingsSet(self, req):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        if req.Data is None:
            return
        rsp_msg = GenericRsp(IsSuccess=True)
        if req.Operation == eThingsOperation.Delete:
            for thing in req.Data:
                thing = Thing.fromCloud(thing)
                del self._Things[thing.ThingKey]
        elif req.Operation == eThingsOperation.Update:
            self.__class__.ThingRevisionNumber = req.RevNum
            for thing in req.Data:
                thing = Thing.fromCloud(thing)
                self._Things[thing.ThingKey] = thing

        PyNodeHelper.LOG.Info(TAG, "Handling finished")
        return rsp_msg

    def HandleActivePortkeysMsg(self, msg):
        PyNodeHelper.LOG.Info(TAG, "Handling started")
        self.activePortKeys = msg.ActivePortKeys
        PyNodeHelper.LOG.Info(TAG, "Handling finished")

    # Helpers
    #  ------------------------------------------------------------------------------------------------------------------
    def SendMsg(self, message):
        PyNodeHelper.LOG.Info(TAG, "Send Msg: " + str(message))
        if self.mqttclient is not None:
            self.mqttclient.SendMsg(message, 2)

    def SendReq(self, message):
        PyNodeHelper.LOG.Info(TAG, "Send Req: " + str(message))
        if self.mqttclient is not None:
            self.mqttclient.SendReq(message, 2)

    def SendRsp(self, message, syncId):
        PyNodeHelper.LOG.Info(TAG, "Send Rsp: " + str(message))
        if self.mqttclient is not None:
            self.mqttclient.SendRsp(message, syncId, 2)

    # update revision number
    def UpdateRevNum(self):
        self.__class__.ThingRevisionNumber  += 1
