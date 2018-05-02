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
        thing = cls()
        thing.ThingKey = dct["ThingKey"] if "ThingKey" in dct else None
        thing.Name = dct["Name"] if "Name" in dct else None
        thing.Type = dct["Type"] if "Type" in dct else None
        thing.BlockType = dct["BlockType"] if "BlockType" in dct else None
        thing.Removable = dct["Removable"] if "Removable" in dct else False
        thing.UIHints = ThingUIHints.fromCloud(dct["UIHints"]) if "UIHints" in dct else None
        ports = dct["Ports"] if "Ports" in dct else None
        thing.Ports = []
        for port in ports:
            p = Port.fromCloud(port)
            if (p is not None):
                thing.Ports.append(p)
        config = dct["Config"] if "Config" in dct else None
        thing.Config = []
        for conf in config:
            cfgparam = ConfigParameter.fromCloud(conf)
            if (cfgparam is not None):
                thing.Config.append(cfgparam)

        return thing


class ConfigParameter(object):
    def __init__(self, Name, Value, Description = ""):
        self.Name = Name
        self.Value = Value
        self.Description = Description

    @classmethod
    def fromCloud(cls,dct):
        if all(field in dct for field in ("Name", "Value")):
            cfg = cls(dct["Name"],dct["Value"])
            cfg.Description = dct["Description"] if "Description" in dct else ""
            return cfg
        else:
            return None

class ThingUIHints(object):
    def __init__(self, IconURI="", Description = ""):
        self.IconURI = IconURI
        self.Description = Description

    @classmethod
    def fromCloud(cls,dct):
        thingUIHints = cls()
        thingUIHints.IconURI = dct["IconURI"] if "IconURI" in dct else ""
        thingUIHints.IconURI = dct["Description"] if "Description" in dct else ""
        return thingUIHints
