using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.NodeLibrary.NodeDiscovery
{
    [StructLayout(LayoutKind.Sequential)]
    public class DiscoveryMessage : YPChannel.Transport.Sockets.IDiscoveryMessageBase
    {
        int _Id;
        public int Id { get { return _Id; } set { _Id = value; } }
        int _Flags;
        public int Flags { get { return _Flags; } set { _Flags = value; } }
        public int MajorVersion;
        public int MinorVersion;
        public int BuildVersion;

        //Info
        public int ProtocolVersion;
        public int YPChannelPort;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Yodiwo.API.Plegma.NodeKey.MaxKeyLength)]
        public byte[] NodeKey = new byte[Yodiwo.API.Plegma.NodeKey.MaxKeyLength];
    }


    public class AssociationRequest
    {
        public string UnsafeNodeKey;
        public byte[] PublicKey;
    }

    public class AssociationResponse
    {
        public DiscoveryMessage DiscoveryMessage;
        public string UnsafeNodeKey;
        public byte[] PublicKey;
    }

    public class AuthenticationRequest
    {
        public string NodeKey;
        public string Hello;
    }

    public class AuthenticationResponse
    {
        public string NodeKey;
        public string Hello;
    }
}
