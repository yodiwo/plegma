import time
from NodeService import *

# -------------------------------------Init Logger --------------------------------------------------------------------
PyNodeHelper.LOG.Init()
# -------------------------------------Read config.json ----------------------------------------------------------------
config = PyNodeHelper.Configuration()
# -------------------------------------Init NodeService ----------------------------------------------------------------
nodeservice = NodeService()
# -------------------------------------Import things -------------------------------------------------------------------
nodeservice.ImportThings(PyNodeHelper.ThingManager.CreateThings(config.nodekey.toString()))
# -------------------------------------Start NodeService ---------------------------------------------------------------
nodeservice.Start(PyNodeHelper.eMessagingProtocol.Mqtt)

time.sleep(15)
# -------------------------------------Create dictionary with port keys ------------------------------------------------
nkey = config.nodekey.toString()
pkey={}
pkey["TextThing-0"] = nodeservice._Things[nkey+"-TextThing"].Ports[0].PortKey
pkey["ButtonThing-0"] = nodeservice._Things[nkey+"-ButtonThing"].Ports[0].PortKey

# -------------------------------------Send PortEventMsg ---------------------------------------------------------------
message = PortEventMsg(1,[PortEvent(pkey["ButtonThing-0"], True, nodeservice.ThingRevisionNumber)])
nodeservice.SendMsg(message )
message = PortEventMsg(1,[PortEvent(pkey["ButtonThing-0"], False, nodeservice.ThingRevisionNumber)])
nodeservice.SendMsg(message)
message = PortEventMsg(1,[PortEvent(pkey["ButtonThing-0"], True, nodeservice.ThingRevisionNumber)])
nodeservice.SendMsg(message )
message = PortEventMsg(1,[PortEvent(pkey["ButtonThing-0"], False, nodeservice.ThingRevisionNumber)])
nodeservice.SendMsg(message )
message = PortEventMsg(1,[PortEvent(pkey["TextThing-0"], "Hello", nodeservice.ThingRevisionNumber)])
nodeservice.SendMsg(message )

# -------------------------------------Teardown NodeService-------------------------------------------------------------
nodeservice.Teardown()
