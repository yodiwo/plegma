## Python Agent
This project contains an example implementation to access the Yodiwo Cloud Services using Plegma API in Python. More information about PlegmaApi can be found [here](https://docs.yodiwo.com/doc/apis/plegma/).

### Requirements
* Python 3.x.
* enum34==1.1.6
* paho-mqtt==1.3.1

### Getting Started

#### Configuration
*  Update [config.json](config.json) file  with node credentials (NodeKey, NodeSecret) and broker's info (MqttBrokerHostname) and specify the target server (ActiveID property).
```
{
    "ActiveID": "dev4",
    "Configs": {
        "tcyan": {
            "NodeKey": "",
            "NodeName": "PyNode",
            "NodeSecret": "",
            "MqttBrokerCertFile": null,
            "MqttBrokerHostname": "tcyan.yodiwo.com",
            "MqttUseSsl": true
        },
		"dev4": {
            "NodeKey": "",
            "NodeName": "PyNode",
            "NodeSecret": "",
            "MqttBrokerCertFile": null,
            "MqttBrokerHostname": "dev4cyan.yodiwo.com",
            "MqttUseSsl": true
        }
	}
}
```

#### Declaration of things
* By default this agent's [ThingManager](PyNodeHelper.py) has 2 things, a 'TextThing' and a 'ButtonThing'
* You can declare new things at CreateThings method of ThingManager class as shown below:
```
# Port (any PortKey as long as it's unique and the ThingId part matches the Thing's)
port = Port(nodekey + "-ButtonThing-0", # PortKey
            "Button",                   # Name
             "Button",                  # Description
             2,                         # ioPortDirection enum   
             4,                         # ePortType enum
             False,                     # initial state
             0,                         # Revision number
             2)                         # ePortConf enum

# Create Thing and update Things dictionary
# ThingKey follows the 'NodeKey-ThingId' notation
# ThingId may be anything unique to this node and must be the basis for its ports' PortKeys
Things[nodekey + "-ButtonThing"] = Thing(
                                        nodekey + "-ButtonThing",       # ThingKey
                                        "Thing's Name",                 # Thing Name
                                        None,                           # List of ConfigParameter objects
                                        [port],                         # List of Ports objects
                                        "com.yodiwo.buttons.default",   # Type
                                        None,                           # BlockType
                                        False,                          # Removable
                                        ThingUIHints(
                                                    IconURI="/Content/img/icons/Generic/thing-genericbutton.png",
                                                    Description="")     # ThingUIHints object, plegma/Thing/ThingUIHints
                                        )
```

#### Examples
#####Python Agent usage example

For a complete example using PythonAgent refer to [PythonAgent/pynode.py](pynode.py) file.

* Initialize Logging:  
```
PyNodeHelper.LOG.Init()
```
* Read configuration:
```
config = PyNodeHelper.Configuration()
```
* Initialize NodeService: 
```
nodeservice = NodeService()
``` 
* Create and Import Things: 
``` 
things = PyNodeHelper.ThingManager.CreateThings(config.nodekey.toString())
nodeservice.ImportThings(things)
``` 
* Start or Teardown NodeService 
```
# start NodeService
nodeservice.Start(PyNodeHelper.eMessagingProtocol.Mqtt)
# Teardown NodeService
nodeservice.Teardown()
```
* Send a PortEventMsg message 
```
# Create PortKey
revNum = nodeservice.ThingRevisionNumber
portkey = config.nodekey.toString() + "ButtonThing-0"
message = PortEventMsg(1, # sequence number
                       [  # List of PortEvent objects
                       PortEvent(pkey["ButtonThing-0"], # portkey
                                 True,                  # State
                                 revNum)                # revision number
                       ]
                       )
nodeservice.SendMsg(message)
``` 

#### Run Instructions

``` 
python3 pynode.py
```
