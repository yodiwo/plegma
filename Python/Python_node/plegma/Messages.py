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

    @classmethod
    def fromCloud(cls,dct):
        syncId = dct["SyncId"] if "SyncId" in dct else None
        payload = dct["Payload"] if "Payload" in dct else None
        flags = dct["Flags"] if "Flags" in dct else None
        payloadSize = dct["PayloadSize"] if "PayloadSize" in dct else None
        msg = cls(SyncId=syncId,Payload=payload,Flags=flags,PayloadSize=payloadSize)
        return msg


class ActivePortKeysMsg(ApiMsg):
    def __init__(self, SeqNo=None, ActivePortKeys=None):
        ApiMsg.__init__(self, SeqNo)
        self.ActivePortKeys = ActivePortKeys

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        activePortKeys = dct["ActivePortKeys"] if "ActivePortKeys" in dct else None
        msg = cls(SeqNo=seqNo, ActivePortKeys=activePortKeys)
        return msg

class GenericRsp(ApiMsg):
    def __init__(self, SeqNo=None, IsSuccess=None, StatusCode=None, Message=""):
        ApiMsg.__init__(self, SeqNo)
        self.IsSuccess = IsSuccess
        self.StatusCode = StatusCode
        self.Message = Message

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        msg = cls(seqNo)
        msg.IsSuccess = dct["IsSuccess"] if "IsSuccess" in dct else None
        msg.StatusCode = dct["StatusCode"] if "StatusCode" in dct else None
        msg.Message = dct["Message"] if "Message" in dct else ""
        return msg

class LoginReq(ApiMsg):
    def __init__(self, SeqNo=None):
        ApiMsg.__init__(self, SeqNo)

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        req = cls(seqNo)
        return req


class LoginRsp(ApiMsg):
    def __init__(self, SeqNo=None, NodeKey=None, SecretKey=None):
        ApiMsg.__init__(self, SeqNo)
        self.NodeKey = NodeKey
        self.SecretKey = SecretKey

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        rsp = cls(seqNo)
        rsp.NodeKey = dct["NodeKey"] if "NodeKey" in dct else None
        rsp.SecretKey = dct["SecretKey"] if "SecretKey" in dct else None
        return rsp

class NodeInfoReq(ApiMsg):
    def __init__(self, SeqNo=None, LatestApiRev=None, AssignedEndpoint=None, ThingsRevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.LatestApiRev = LatestApiRev
        self.AssignedEndpoint = AssignedEndpoint
        self.ThingsRevNum = ThingsRevNum

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        req = cls(seqNo)
        req.LatestApiRev = dct["LatestApiRev"] if "LatestApiRev" in dct else None
        req.AssignedEndpoint = dct["AssignedEndpoint"] if "AssignedEndpoint" in dct else None
        req.ThingsRevNum = dct["ThingsRevNum"] if "ThingsRevNum" in dct else None
        return req

class NodeInfoRsp(ApiMsg):
    def __init__(self, SeqNo=None, Name=None, Type=None, Capabilities=None, ThingTypes=None, ThingsRevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.Name = Name
        self.Type = Type
        self.Capabilities = Capabilities
        self.ThingTypes = ThingTypes
        self.ThingsRevNum = ThingsRevNum

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        rsp = cls(seqNo)
        rsp.Name = dct["Name"] if "Name" in dct else None
        rsp.Type = dct["Type"] if "Type" in dct else None
        rsp.Capabilities = dct["Capabilities"] if "Capabilities" in dct else None
        rsp.ThingTypes = dct["ThingTypes"] if "ThingTypes" in dct else None
        rsp.ThingsRevNum = dct["ThingsRevNum"] if "ThingsRevNum" in dct else None
        return rsp

class PortEvent(object):
    def __init__(self, PortKey, State, RevNum=0, Timestamp=0):
        self.PortKey = PortKey
        self.State = State
        self.RevNum = RevNum
        self.Timestamp = Timestamp

    @classmethod
    def fromCloud(cls,dct):
        if all(field in dct for field in ("PortKey", "State")):
            event = cls(dct["PortKey"], dct["State"])
            event.RevNum = dct["RevNum"] if "RevNum" in dct else 0
            event.Timestamp = dct["Timestamp"] if "Timestamp" in dct else 0
            return event
        else:
            # missing attributes, object cannot be created
            return None

class PortEventMsg(ApiMsg):
    def __init__(self, SeqNo=None, PortEvents=None):
        ApiMsg.__init__(self, SeqNo)
        self.PortEvents = PortEvents

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        msg = cls(seqNo)
        msg.PortEvents = dct["PortEvents"] if "PortEvents" in dct else None
        return msg

class PortState(object):
    def __init__(self, PortKey, State, RevNum, IsDeployed):
        self.PortKey = PortKey
        self.State = State
        self.RevNum = RevNum
        self.IsDeployed = IsDeployed

    @classmethod
    def fromCloud(cls,dct):
        if all(field in dct for field in ("PortKey", "State", "RevNum", "IsDeployed")):
            state = cls(dct["PortKey"], dct["State"], dct["RevNum"], dct["IsDeployed"])
            return state
        else:
            # missing attributes, object cannot be created
            return None

    def toJson(self):
        return json.dumps(self, default=lambda o: o.__dict__)


class PortStateGet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, PortKeys=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.PortKeys = PortKeys

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        msg = cls(seqNo)
        msg.Operation = dct["Operation"] if "Operation" in dct else None
        msg.PortKeys = dct["PortKeys"] if "PortKeys" in dct else None
        return msg

class PortStateSet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, PortStates=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.PortStates = PortStates

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        msg = cls(seqNo)
        msg.Operation = dct["Operation"] if "Operation" in dct else None
        msg.PortStates = dct["PortStates"] if "PortStates" in dct else None
        return msg

class ThingsGet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, ThingKey=None, Key=None, RevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.ThingKey = ThingKey
        self.Key = Key
        self.RevNum = RevNum

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        msg = cls(seqNo)
        msg.Operation = dct["Operation"] if "Operation" in dct else None
        msg.ThingKey = dct["ThingKey"] if "ThingKey" in dct else None
        msg.Key = dct["Key"] if "Key" in dct else None
        msg.RevNum = dct["RevNum"] if "RevNum" in dct else None
        return msg

class ThingsSet(ApiMsg):
    def __init__(self, SeqNo=None, Operation=None, Status=None, Data=None, RevNum=None):
        ApiMsg.__init__(self, SeqNo)
        self.Operation = Operation
        self.Status = Status
        self.Data = Data
        self.RevNum = RevNum

    @classmethod
    def fromCloud(cls,dct):
        seqNo = dct["SeqNo"] if "SeqNo" in dct else None
        msg = cls(seqNo)
        msg.Operation = dct["Operation"] if "Operation" in dct else None
        msg.Status = dct["Status"] if "Status" in dct else None
        msg.Data = dct["Data"] if "Data" in dct else None
        msg.RevNum = dct["RevNum"] if "RevNum" in dct else None
        return msg

class NodeStatusChangedReq(ApiMsg):
    def __init__(self, SeqNo=None, NewStatus=None, ReasonCode=None, Message=None):
        ApiMsg.__init__(self, SeqNo)
        self.NewStatus = NewStatus
        self.ReasonCode = ReasonCode
        self.Message = Message
