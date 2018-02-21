using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.mNode.Plugins.Bridge_OZWave
{
    #region OZW Notifications enums

    public enum NotificationType
    {
        Type_ValueAdded = 0,                /**< A new node value has been added to OpenZWave's list. These notifications occur after a node has been discovered, and details of its command classes have been received.  Each command class may generate one or more values depending on the complexity of the item being represented.  */
        Type_ValueRemoved,                  /**< A node value has been removed from OpenZWave's list.  This only occurs when a node is removed. */
        Type_ValueChanged,                  /**< A node value has been updated from the Z-Wave network and it is different from the previous value. */
        Type_ValueRefreshed,                /**< A node value has been updated from the Z-Wave network. */
        Type_Group,                         /**< The associations for the node have changed. The application should rebuild any group information it holds about the node. */
        Type_NodeNew,                       /**< A new node has been found (not already stored in zwcfg*.xml file) */
        Type_NodeAdded,                     /**< A new node has been added to OpenZWave's list.  This may be due to a device being added to the Z-Wave network, or because the application is initializing itself. */
        Type_NodeRemoved,                   /**< A node has been removed from OpenZWave's list.  This may be due to a device being removed from the Z-Wave network, or because the application is closing. */
        Type_NodeProtocolInfo,              /**< Basic node information has been received, such as whether the node is a listening device, a routing device and its baud rate and basic, generic and specific types. It is after this notification that you can call Manager::GetNodeType to obtain a label containing the device description. */
        Type_NodeNaming,                    /**< One of the node names has changed (name, manufacturer, product). */
        Type_NodeEvent,                     /**< A node has triggered an event.  This is commonly caused when a node sends a Basic_Set command to the controller.  The event value is stored in the notification. */
        Type_PollingDisabled,               /**< Polling of a node has been successfully turned off by a call to Manager::DisablePoll */
        Type_PollingEnabled,                /**< Polling of a node has been successfully turned on by a call to Manager::EnablePoll */
        Type_SceneEvent,                    /**< Scene Activation Set received */
        Type_CreateButton,                  /**< Handheld controller button event created */
        Type_DeleteButton,                  /**< Handheld controller button event deleted */
        Type_ButtonOn,                      /**< Handheld controller button on pressed event */
        Type_ButtonOff,                     /**< Handheld controller button off pressed event */
        Type_DriverReady,                   /**< A driver for a PC Z-Wave controller has been added and is ready to use.  The notification will contain the controller's Home ID, which is needed to call most of the Manager methods. */
        Type_DriverFailed,                  /**< Driver failed to load */
        Type_DriverReset,                   /**< All nodes and values for this driver have been removed.  This is sent instead of potentially hundreds of individual node and value notifications. */
        Type_EssentialNodeQueriesComplete,  /**< The queries on a node that are essential to its operation have been completed. The node can now handle incoming messages. */
        Type_NodeQueriesComplete,           /**< All the initialization queries on a node have been completed. */
        Type_AwakeNodesQueried,             /**< All awake nodes have been queried, so client application can expected complete data for these nodes. */
        Type_AllNodesQueriedSomeDead,       /**< All nodes have been queried but some dead nodes found. */
        Type_AllNodesQueried,               /**< All nodes have been queried, so client application can expected complete data. */
        Type_Notification,                  /**< An error has occurred that we need to report. */
        Type_DriverRemoved,                 /**< The Driver is being removed. (either due to Error or by request) Do Not Call Any Driver Related Methods after receiving this call */
        Type_ControllerCommand,             /**< When Controller Commands are executed, Notifications of Success/Failure etc are communicated via this Notification
													* Notification::GetEvent returns Driver::ControllerState and Notification::GetNotification returns Driver::ControllerError if there was a error */
        Type_NodeReset                      /**< The Device has been reset and thus removed from the NodeList in OZW */
    };

    public enum NotificationCode
    {
        MsgComplete = 0,           /**< Completed messages */
        Timeout,                   /**< Messages that timeout will send a Notification with this code. */
        NoOperation,               /**< Report on NoOperation message sent completion  */
        Awake,                     /**< Report when a sleeping node wakes up */
        Sleep,                     /**< Report when a node goes to sleep */
        Dead,                      /**< Report when a node is presumed dead */
        Alive                      /**< Report when a node is revived */
    };

    public enum ValueGenre
    {
        ValueGenre_Basic = 0,       /**< The 'level' as controlled by basic commands.  Usually duplicated by another command class. */
        ValueGenre_User,            /**< Basic values an ordinary user would be interested in. */
        ValueGenre_Config,          /**< Device-specific configuration parameters.  These cannot be automatically discovered via Z-Wave, and are usually described in the user manual instead. */
        ValueGenre_System,          /**< Values of significance only to users who understand the Z-Wave protocol */
        ValueGenre_Count            /**< A count of the number of genres defined.  Not to be used as a genre itself. */
    };

    public enum ValueType
    {
        ValueType_Bool = 0,                 /**< Boolean, true or false */
        ValueType_Byte,                     /**< 8-bit unsigned value */
        ValueType_Decimal,                  /**< Represents a non-integer value as a string, to avoid floating point accuracy issues. */
        ValueType_Int,                      /**< 32-bit signed value */
        ValueType_List,                     /**< List from which one item can be selected */
        ValueType_Schedule,                 /**< Complex type used with the Climate Control Schedule command class */
        ValueType_Short,                    /**< 16-bit signed value */
        ValueType_String,                   /**< Text string */
        ValueType_Button,                   /**< A write-only value that is the equivalent of pressing a button to send a command to a device */
        ValueType_Raw,                      /**< A collection of bytes */
        ValueType_Max = ValueType_Raw       /**< The highest-number type defined.  Not to be used as a type itself. */
    };

    public enum CommandClassId
    {
        ALARM = 113,
        APPLICATION_STATUS = 34,
        ASSOCIATION = 133,
        ASSOCIATION_COMMAND_CONFIGURATION = 155,
        BASIC = 32,
        BASIC_WINDOW_COVERING = 80,
        BATTERY = 128,
        CENTRAL_SCENE = 91,
        CLIMATE_CONTROL_SCHEDULE = 70,
        CLOCK = 129,
        CONFIGURATION = 112,
        CONTROLLER_REPLICATION = 33,
        CRC_16_ENCAP = 86,
        DOOR_LOCK = 98,
        DOOR_LOCK_LOGGING = 76,
        ENERGY_PRODUCTION = 144,
        HAIL = 130,
        INDICATOR = 135,
        LANGUAGE = 137,
        LOCK = 118,
        MANUFACTURER_SPECIFIC = 114,
        METER = 50,
        METER_PULSE = 53,
        MULTI_CHANNEL_ASSOCIATION = 142,
        MULTI_CMD = 143,
        MULTI_INSTANCE = 96,
        NODE_NAMING = 119,
        NO_OPERATION = 0,
        POWERLEVEL = 115,
        PROPRIETARY = 136,
        PROTECTION = 117,
        SCENE_ACTIVATION = 43,
        SECURITY = 152,
        SENSOR_ALARM = 156,
        SENSOR_BINARY = 48,
        SENSOR_MULTILEVEL = 49,
        SWITCH_ALL = 39,
        SWITCH_BINARY = 37,
        SWITCH_MULTILEVEL = 38,
        SWITCH_TOGGLE_BINARY = 40,
        SWITCH_TOGGLE_MULTILEVEL = 41,
        THERMOSTAT_FAN_MODE = 68,
        THERMOSTAT_FAN_STATE = 69,
        THERMOSTAT_MODE = 64,
        THERMOSTAT_OPERATING_STATE = 66,
        THERMOSTAT_SETPOINT = 67,
        TIME_PARAMETERS = 139,
        USER_CODE = 99,
        VERSION = 134,
        WAKE_UP = 132,
    };

    #endregion

    #region Yodikit Devices Explicit

    public enum DevicesName
    {
        MicroSmartPlug = 0,         // nodeOn plug
        Powerplug12A,               // neo coolcam plug
        DoorWindowDetector,         // neo coolcam door/window detector
        FGK10xDoorOpeningSensor,    // Fibaro door/window sensor
        ZXT_120EU,                  // ZXT-120 zwave-To-AC IR extender
        DanalockV2BTZE,             // Danalock
        ZW100MultiSensor6,          // Aeotec Multisensor 6
        FGWPEFWallPlug,             // Fibaro Smart Plug
    };

    public enum PlugMeteringType
    {
        none = 0,
        kWh,        //energy
        W,          // power
        A,          //current
        V,          //voltage
    };

    #endregion

    #region OZW - ZWave Plugin Communication Structs

    public struct MsgFromCpp
    {
        public NotificationType NotificationType;
        public NotificationCode NotificationCode;
        public UInt32 homeID;
        public Byte nodeID;
        public Byte groupId;
        public string name;
        public string currentValue;
        public ValueGenre genre;
        public CommandClassId commandClassId;
        public Byte instance;
        public Byte valueIndex;
        public ValueType type;
        public string label;
    }

    public struct ValueID
    {
        public UInt32 homeID;
        public Byte nodeID;
        public ValueGenre genre;
        public CommandClassId commandClassId;
        public Byte instance;
        public Byte valueIndex;
        public ValueType type;
        public string label;
    }

    #endregion
}