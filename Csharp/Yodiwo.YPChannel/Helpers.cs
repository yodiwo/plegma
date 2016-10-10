using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Yodiwo.YPChannel
{
    public static partial class Helpers
    {
        #region Lookup Dictionaries
        public static IReadOnlyDictionary<Type, MessageIEValueType> Type2Enum = new Dictionary<Type, MessageIEValueType>()
        {
            { typeof(object) , MessageIEValueType.Object },
            { typeof(String) , MessageIEValueType.String },
            { typeof(Byte) , MessageIEValueType.Byte },
            { typeof(Boolean) , MessageIEValueType.Boolean },
            { typeof(Int16) , MessageIEValueType.Int16 },
            { typeof(Int32) , MessageIEValueType.Int32 },
            { typeof(Int64) , MessageIEValueType.Int64 },
            { typeof(UInt16) , MessageIEValueType.UInt16 },
            { typeof(UInt32) , MessageIEValueType.UInt32 },
            { typeof(UInt64) , MessageIEValueType.UInt64 },
            { typeof(Single) , MessageIEValueType.Single },
            { typeof(Double) , MessageIEValueType.Double },
        };

        public static IReadOnlyDictionary<MessageIEValueType, Type> Enum2Type = new Dictionary<MessageIEValueType, Type>()
        {
            { MessageIEValueType.Object ,typeof(object) },
            { MessageIEValueType.String, typeof(String) },
            { MessageIEValueType.Byte, typeof(Byte) },
            { MessageIEValueType.Boolean, typeof(Boolean) },
            { MessageIEValueType.Int16, typeof(Int16) },
            { MessageIEValueType.Int32, typeof(Int32)  },
            { MessageIEValueType.Int64, typeof(Int64) },
            { MessageIEValueType.UInt16, typeof(UInt16)  },
            { MessageIEValueType.UInt32, typeof(UInt32) },
            {  MessageIEValueType.UInt64, typeof(UInt64) },
            { MessageIEValueType.Single, typeof(Single)  },
            {  MessageIEValueType.Double, typeof(Double) },
        };
        #endregion
    }

}
