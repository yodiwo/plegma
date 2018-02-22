from plegma.Messages import *


s_LoginReq = "loginreq"
s_LoginRsp = "loginrsp"
s_NodeInfoReq = "nodeinforeq"
s_NodeInfoRsp = "nodeinforsp"
s_ThingsGet = "thingsget"
s_ThingsSet = "thingsset"
s_PortEventMsg = "porteventmsg"
s_PortStateGet = "portstateget"
s_PortStateSet = "portstateset"
s_ActivePortKeysMsg = "activeportkeysmsg"
s_GenericRsp = "genericrsp"


class PlegmaApi:
     def __init__(self):
         self.ApiMsgNames = {
             type(LoginReq()): s_LoginReq,
             type(LoginRsp()): s_LoginRsp,
             type(NodeInfoReq()): s_NodeInfoReq,
             type(NodeInfoRsp()): s_NodeInfoRsp,
             type(ThingsGet()): s_ThingsGet,
             type(ThingsSet()): s_ThingsSet,
             type(PortEventMsg()): s_PortEventMsg,
             type(PortStateGet()): s_PortStateGet,
             type(PortStateSet()): s_PortStateSet,
             type(ActivePortKeysMsg()): s_ActivePortKeysMsg,
             type(GenericRsp()): s_GenericRsp
         }
         self.ApiMsgNamesToTypes = {
             s_LoginReq: type(LoginReq()),
             s_LoginRsp: type(LoginRsp()),
             s_NodeInfoReq: type(NodeInfoReq()),
             s_NodeInfoRsp: type(NodeInfoRsp()),
             s_ThingsGet: type(ThingsGet()),
             s_ThingsSet: type(ThingsSet()),
             s_PortEventMsg: type(PortEventMsg()),
             s_PortStateGet: type(PortStateGet()),
             s_PortStateSet: type(PortStateSet()),
             s_ActivePortKeysMsg: type(ActivePortKeysMsg()),
             s_GenericRsp: type(GenericRsp())
         }



