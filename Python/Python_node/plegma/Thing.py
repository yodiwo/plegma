from  plegma.Port import Port

class Thing(object):
    def __init__(self, ThingKey=None, Name=None, Config=None, Ports=None, Type=None, BlockType=None, Removable=False, UIHints=None, ReadonlyInfo = "", Hierarchy="", ConfFlags=0, RESTUri=""):
        self.ThingKey = ThingKey
        self.Name = Name
        self.Config = Config # List of ConfigParameters objects
        self.Ports = Ports # List of Ports objects
        self.Type = Type
        self.BlockType = BlockType
        self.Removable = Removable
        self.UIHints = UIHints # ThingUIHints object

    @classmethod
    def fromCloud(cls,dct):
        thing = cls(**dct)
        thing.UIHints = ThingUIHints(**thing.UIHints)
        ports = thing.Ports
        thing.Ports = []
        for port in ports:
            thing.Ports.append(Port(**port))
        config = thing.Config
        thing.Config = []
        for conf in config:
            thing.Config.append(ConfigParameter(**conf))

        return thing


class ConfigParameter(object):
    def __init__(self, Name, Value, Description = ""):
        self.Name = Name
        self.Value = Value
        self.Description = Description

class ThingUIHints(object):
    def __init__(self, IconURI="", Description = ""):
        self.IconURI = IconURI
        self.Description = Description
