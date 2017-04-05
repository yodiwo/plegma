using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.mNode.Plugins.CPP
{
    // --------------------------------------------------------------------------------------------
    public enum MsgType
    {
        Undefined = 0,
        SendPortEvent = 1
    }
    // --------------------------------------------------------------------------------------------
    public struct Msg
    {
        public MsgType Type;

        /// Json form of the actual message
        public String Message;
    }
    // --------------------------------------------------------------------------------------------
    public class SendPortEvent
    {
        /// <summary>
        /// Globally unique Key string of this Thing
        /// </summary>
        public String ThingKey;

        /// <summary>Globally unique string identifying this port; Construct it using the <see cref="PortKey"/> constructor</summary>
        public String PortKey;

        /// <summary>
        /// New State
        /// </summary>
        public String State;
    }
    // --------------------------------------------------------------------------------------------
}
