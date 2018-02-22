import json
from plegma.EnumsApi import *


class ApiMsg(object):
    def __init__(self, SeqNo=None):
        self.SeqNo = SeqNo

    def toJson(self):
        return json.dumps(self, default=lambda o: o.__dict__)

class WrapperMsg(object):
    def __init__(self, SyncId, Payload, Flags, PayloadSize = None):
        self.SyncId = SyncId
        self.Payload = Payload
        self.Flags = Flags
        self.PayloadSize = PayloadSize

    def isRequest(self):
        return True if self.Flags == eMsgFlags.Request else False

    def isResponse(self):
        return True if self.Flags == eMsgFlags.Response else False

    def toJson(self):
        return json.dumps(self, default=lambda o: o.__dict__)


class MqttMsg(WrapperMsg):
    def __init__(self, SyncId=None, Payload=None, Flags=None, PayloadSize=None):
        WrapperMsg.__init__(self, SyncId, Payload, Flags, PayloadSize)


class ActivePortKeysMsg(ApiMsg):
    def __init__(self, SeqNo=None, ActivePortKeys=None):
        ApiMsg.__init__(self, SeqNo)
        self.ActivePortKeys = ActivePortKeys


class GenericRsp(ApiMsg):
    def __init__(self, SeqNo=None, IsSuccess=None, StatusCode=None, Message=""):
        ApiMsg.__init__(self, SeqNo)
        self.IsSuccess = IsSuccess
        self.StatusCode = StatusCode
        self.Message = Message


class LoginReq(ApiMsg):
    def __init__(self, SeqNo=None):
        ApiMsg.__init__(self, SeqNo)


class LoginRsp(ApiMsg):
    def __init__(self, SeqNo=None, NodeKey=None, SecretKey=None):
        ApiMsg.__init__(self, SeqNo)
        self.NodeKey = NodeKey
        self.SecretKey = SecretKey


class NodeInfoReq(ApiMsg):
    def __init__(self, SeqNo=None, LatestApiRev=None, AssignedEndpoint=None, ThingsRevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.LatestApiRev = LatestApiRev
        self.AssignedEndpoint = AssignedEndpoint
        self.ThingsRevNum = ThingsRevNum


class NodeInfoRsp(ApiMsg):
    def __init__(self, SeqNo=None, Name=None, Type=None, Capabilities=None, ThingTypes=None, ThingsRevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.Name = Name
        self.Type = Type
        self.Capabilities = Capabilities
        self.ThingTypes = ThingTypes
        self.ThingsRevNum = ThingsRevNum


class PortEvent(ApiMsg):
    def __init__(self, PortKey, State, RevNum, SeqNo=None, Timestamp=None):
        ApiMsg.__init__(self, SeqNo)
        self.PortKey = PortKey
        self.State = State
        self.RevNum = RevNum

class PortEventMsg(ApiMsg):
    def __init__(self, SeqNo=None, PortEvents=None):
        ApiMsg.__init__(self, SeqNo)
        self.PortEvents = PortEvents


class PortState(object):
    def __init__(self, PortKey, State, RevNum, IsDeployed):
        self.PortKey = PortKey
        self.State = State
        self.RevNum = RevNum
        self.IsDeployed = IsDeployed

    def toJson(self):
        return json.dumps(self, default=lambda o: o.__dict__)


class PortStateGet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, PortKeys=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.PortKeys = PortKeys


class PortStateSet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, PortStates=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.PortStates = PortStates


class ThingsGet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, ThingKey=None, Key=None, RevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.ThingKey = ThingKey
        self.Key = Key
        self.RevNum = RevNum


class ThingsSet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, Status=None, Data=None, RevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.Status = Status
        self.Data = Data
        self.RevNum = RevNum


class NodeStatusChangedReq(ApiMsg):
    def __init__(self, SeqNo=None, NewStatus=None, ReasonCode=None, Message=None):
        ApiMsg.__init__(self, SeqNo)
        self.NewStatus = NewStatus
        self.ReasonCode = ReasonCode
        self.Message = Message
