using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;
using Yodiwo.API.Spike;
using Yodiwo.Tools;

namespace Yodiwo.Spike
{
    public class SpikeMessage
    {
        #region variables
        public static readonly Encoding Encoding = System.Text.Encoding.UTF8;

        public SpikeMessageHeader Header = new SpikeMessageHeader();

        public List<SpikeContainerTlv> Tlvs = new List<SpikeContainerTlv>();
        #endregion

        #region functions
        public byte[] GetBytes()
        {
            var hsize = Marshal.SizeOf(typeof(SpikeMessageHeader));
            var tlvbytes = Tlvs.Select(x => x.ToBytes(Endianness.BigEndian)).ToList();
            var tlvsize = tlvbytes.Sum(x => x.Length);

            // tlvsize in header is invalid until now... maybe fix this?
            Header.TlvsLength = (UInt16)tlvsize;

            var headerBytes = Marshalling.ToBytes(Header, Endianness.BigEndian);
            return tlvbytes.Aggregate((IEnumerable<byte>)headerBytes, (current, tlv) => current.Concat(tlv)).ToArray();
        }

        public static SpikeMessage FromBytes(byte[] bytes)
        {
            SpikeMessage spike = new SpikeMessage();
            var offset = Marshalling.ToObject(bytes, 0, out spike.Header, Endianness.BigEndian);
            if (offset < 0)
                return null;

            while (bytes.Length - offset > 4)
            {
                SpikeContainerTlv tlv;
                var newoff = SpikeContainerTlv.FromBytes(bytes, offset, out tlv, Endianness.BigEndian);
                if (newoff < 0)
                    return spike;
                offset += newoff;
                spike.Tlvs.Add(tlv);
            }

            return spike;
        }

        public byte[] TlvHeaderBytes(TlvTypes type, UInt16 length)
        {
            return BigBitConverter.GetBytes((UInt16)type).Concat(BigBitConverter.GetBytes(length)).ToArray();
        }

        public byte[] SetValueHeaderBytes(UInt32 thingId, byte portId)
        {
            return BigBitConverter.GetBytes(thingId).Concat(new byte[] { portId, 0, 0, 0 }).ToArray();
        }
        #endregion
    }

}
