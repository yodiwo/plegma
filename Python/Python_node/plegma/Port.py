class Port(object):
    def __init__(self, PortKey, Name, Description, ioDirection, Type, State, RevNum, ConfFlags, Color="", LastUpdatedTimestamp="", PortModelId="", Size=None, Semantics=None, IsOutputPort=None, IsInputPort=None):
        self.PortKey = PortKey
        self.Name = Name
        self.Description = Description
        self.ioDirection = ioDirection
        self.Type = Type
        self.State = State
        self.RevNum = RevNum
        self.ConfFlags = ConfFlags

    @classmethod
    def fromCloud(cls,dct):
        if all(field in dct for field in ("PortKey", "Name", "Description", "ioDirection", "Type", "State", "RevNum", "ConfFlags")):
            port = cls(dct["PortKey"], dct["Name"], dct["Description"], dct["ioDirection"], dct["Type"], dct["State"], dct["RevNum"], dct["ConfFlags"])
            return port
        else:
            return None
