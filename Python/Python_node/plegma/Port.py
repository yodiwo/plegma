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
