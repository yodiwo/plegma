using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.YPChannel
{
    public interface ISerializer
    {
        byte[] Pack_YPMessagePacked(Channel.YPMessagePacked obj);
        Channel.YPMessagePacked Unpack_YPMessagePacked(Stream memStream);

        byte[] Pack(object obj);
        object Unpack(Type objType, byte[] buffer);
    }
}
