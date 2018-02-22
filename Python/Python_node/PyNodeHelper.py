import json
import logging
from enum import IntEnum

from plegma.Keys import NodeKey, UserKey
from plegma.Port import Port
from plegma.Thing import Thing, ConfigParameter, ThingUIHints


class Configuration(object):
    __instance = None

    def __init__(self):
        if Configuration.__instance is None:
            Configuration.__instance = Configuration.__impl()
        self.__dict__['_Singleton__instance'] = Configuration.__instance

    def __getattr__(self, attr):
        return getattr(self.__instance, attr)

    def __setattr__(self, attr, value):
        return setattr(self.__instance, attr, value)

    class __impl(object):
        def __init__(self):
            self.ReadConfig()

        def ReadConfig(self):
            with open('config.json') as json_data:
                configjson = json.load(json_data)
                activeid = configjson["ActiveID"]
                configs = configjson["Configs"]
                activeconfig = configs[activeid]
                self.mqttbrokeraddress = activeconfig["MqttBrokerHostname"]
                self.mqttbrokercert = activeconfig["MqttBrokerCertFile"]
                self.mqttbrokerusessl = activeconfig["MqttUseSsl"]
                self.nodesecretkey = activeconfig["NodeSecret"]
                self.nodename = activeconfig["NodeName"]
                self.nodekey = NodeKey()
                self.nodekey.setKey(activeconfig["NodeKey"])
            self.userkey = UserKey(self.nodekey.key)

class eMessagingProtocol(IntEnum):
    Unknown, Mqtt = range(2)

class ThingManager(object):
    @staticmethod
    def CreateThings(nodekey):
        # Dummy things (Button and Text)
        Things = {}
        Things[nodekey + "-TextThing"] = Thing(nodekey + "-TextThing",
                                               "Python Text",
                                               [ConfigParameter("Python Text", "test")],
                                               [Port(nodekey + "-TextThing-0",
                                                     "Text",
                                                     "Simple Text", 2, 6,
                                                     "", 0, 2),
                                                Port(nodekey + "-TextThing-1",
                                                     "Text",
                                                     "Simple Text", 3, 6,
                                                     "", 0, 2)
                                                ],
                                               "com.yodiwo.text.default", None, False,
                                               ThingUIHints(
                                                   IconURI="/Content/img/icons/Generic/thing-text.png",
                                                   Description=""))
        Things[nodekey + "-ButtonThing"] = Thing(nodekey + "-ButtonThing",
                                                "Python Button",
                                                None,
                                                [Port(nodekey + "-ButtonThing-0",
                                                      "Button",
                                                      "Button", 2, 4,
                                                      False, 0, 2)],
                                                "com.yodiwo.buttons.default", None, False,
                                                ThingUIHints(
                                                    IconURI="/Content/img/icons/Generic/thing-genericbutton.png",
                                                    Description=""))
        return Things

class Converter(object):
    @staticmethod
    def PortkeyToThingkey(pk):
        return pk.split("-")[0] + "-" + pk.split("-")[1] + "-" + pk.split("-")[2]

class LOG(object):
    @staticmethod
    def Init():
        logging.basicConfig(filename='pynode.log', format='%(asctime)s: %(name)s : %(levelname)s : %(message)s',
                            datefmt='%m/%d/%Y %I:%M:%S %p', level=logging.ERROR)
        logging.info("***** Started *****")

    @staticmethod
    def Info(TAG, msg):
        logging.info("--> " + TAG + " *** " + msg)
        print("--> " + TAG + " *** " + msg)

    @staticmethod
    def Warn(TAG, msg):
        logging.warning("--> " + TAG + " *** " + msg)
        print("--> " + TAG + " *** " + msg)

    @staticmethod
    def Error(TAG, msg):
        logging.error("--> " + TAG + " *** " + msg)
        print("--> " + TAG + " *** " + msg)
