using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.YPChannel.Transport.Basic
{
    public abstract class BasicChannel : Channel
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        protected internal PipeStream pipe = new PipeStream();
        //------------------------------------------------------------------------------------------------------------------------
        public override string RemoteIdentifier { get { return ""; } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public BasicChannel(Protocol[] Protocols)
            : base(Protocols, ChannelRole.Unkown, ChannelSerializationMode.MessagePack, true)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override YPMessagePacked _readPackedMessage()
        {
            /*
            //check connection
            try
            {
                return EncapsulationSerializer.Unpack(stream);
            }
            catch { return null; }
            */
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void _sendPackedMessage(YPMessagePacked msg)
        {
            /*
            lock (this)
            {
                //check connection
                if (stream == null || !_IsConnected)
                    return;

                try
                {
                    //pack and send
                    var enc_msg = EncapsulationSerializer.PackSingleObject(msg);
                    stream.BeginWrite(enc_msg, 0, enc_msg.Length, null, null);
                }
                catch { Disconnect(); }
            }*/
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void onClose()
        {
            base.onClose();

            lock (this)
            {
                pipe.Dispose();
                pipe = null;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
