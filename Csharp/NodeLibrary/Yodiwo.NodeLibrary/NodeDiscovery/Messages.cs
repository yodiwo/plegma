using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.NodeLibrary.NodeDiscovery
{
    [StructLayout(LayoutKind.Sequential)]
    public class DiscoveryMessage : YPChannel.Transport.Sockets.DiscoveryMessageBase
    {
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
