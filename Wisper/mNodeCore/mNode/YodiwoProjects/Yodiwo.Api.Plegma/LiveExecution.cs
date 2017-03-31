using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Logic
{
    public class LiveExecutionBlockSolve
    {
        public object Value;
        public int Index;
        public bool IsConnected;
        public bool IsDirty;
        public bool IsInput;
        public bool IsOutput;
        public bool IsTouched;
        public string Extra;
    }

    public class LiveExecutionSolveIo : LiveExecutionBlockSolve
    {
        public string IoName;
    }

    public class LiveExecutionSolve
    {
        public BlockKey BlockKey;
        public GraphDescriptorKey GraphDescriptorKey;
        public int OriginalUID;
        public LiveExecutionSolveIo[] Ios;
    }

    public class LiveExecutionPortState : LiveExecutionBlockSolve
    {
        public PortKey PortKey;
        public string RevNum;
        public string LastUpdatedTimestamp;
        public string Color;
        public int Size;
    }

    public class LiveExecutionPortStates
    {
        public LiveExecutionPortState[] PortStates;
    }

    public struct ListenerKey
    {
        public enum eListenerType
        {
            WebsocketListener,
            WarlockListener
        }
        public eListenerType Type;
        public string ListenerId;

        public ListenerKey(eListenerType type, string id) { Type = type; ListenerId = id; }
    }

}
