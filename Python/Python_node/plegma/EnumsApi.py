from enum import IntEnum


class eDriverType(IntEnum):
    Unknown, LCD = range(2)


class ioPortDirection(IntEnum):
    Undefined, InputOutput, Output, Input = range(4)


class ePortType(IntEnum):
    Undefined, Integer, Decimal, DecimalHigh, Boolean, Color, String, VideoDescriptor, AudioDescriptor, \
    BinaryResourceDescriptor, I2CDescriptor, JsonString, IncidentDescriptor, Timestamp = range(14)


class ePortConf(IntEnum):
    portConfNone = 0
    PropagateAllEvents, IsTrigger, DoNotNormalize, SupressIdenticalEvents = [2**i for i in range(4)]


class BinaryResourceContentType(IntEnum):
    Undefined, Data, Text, Image, Audio, Video = range(6)


class BinaryResourceLocationType(IntEnum):
    Undefined, Http, RedisDB = range(3)


class RestServiceType(IntEnum):
    Undefined, Dropbox, Pastebin, GoogleDrive, Yodiwo = range(5)


class ImageFileFormat(IntEnum):
    Undefined, PNG, TIFF, GIF, BMP, SVG = range(6)


class ImageType(IntEnum):
    Raster, Vector = range(2)


class eConnectionFlags(IntEnum):
    connFlagsNone = 0
    CreateNewEndpoint, IsMasterEndpoint, KillExistingNodeLinks = [2**i for i in range(3)]


class eA2mcuCtrlType(IntEnum):
    Reset, SetValue, WriteDriconf = range(3)


class eNodeCapa(IntEnum):
    nodeCapaNone = 0
    SupportsGraphSolving, Scannable, IsWarlock, IsShellNode = [2**i for i in range(4)]


class eNodeSyncOperation(IntEnum):
    GetEndpoints, SetEndpoint = range(1,3)


class eNodeType(IntEnum):
    Unknown, Generic, EndpointSingle, TestGateway, TestEndpoint, WSEndpoint, Android, \
    iOS, SmartThingsEndPoint, mNode, Dashboard, LoRa, Impact = range(13)
    WSSample = 200
    RestSample = 201
    Virtual = 202
    Yodikit = 203


class ePortStateOperation(IntEnum):
    Invalid, SpecificKeys, ActivePortStates, AllPortStates = range(4)


class eThingsOperation(IntEnum):
    Invalid, Update, Overwrite, Delete, Get, Scan, Sync = range(7)


class eUnpairReason(IntEnum):
    Unknown, InvalidOperation, UserRequested, TooManyAuthFailures = range(4)

class eMsgFlags(IntEnum):
    Message, Request, Response = range(3)

class eNodeNewStatus(IntEnum):
    Unpaired, Disabled, Enabled = range(3)
