using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Lame;
using System.IO;

namespace Yodiwo.Media.Audio.Sink.MP3Server
{
    public class Mp3Writer : IDisposable
    {

        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        private LameMP3FileWriter lameMp3wr;
        private Stream ns;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public Mp3Writer(Stream stream)
        {
            this.ns = stream;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions

        //------------------------------------------------------------------------------------------------------------------------

        public void WriteHeader()
        {
            Write(
@"HTTP/1.1 200 OK
Content-Type: audio/mpeg

");
            this.ns.Flush();
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void WriteMp3Header()
        {
            lock (this)
                lameMp3wr = new LameMP3FileWriter(this.ns, new NAudio.Wave.WaveFormat(8000, 2), 128);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Write(string text)
        {
            byte[] data = BytesOf(text);
            this.ns.Write(data, 0, data.Length);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Write(byte[] mp3data)
        {
            lock (this)
                lameMp3wr.Write(mp3data, 0, mp3data.Length);
        }

        //------------------------------------------------------------------------------------------------------------------------

        private static byte[] BytesOf(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            try
            {
                if (this.ns != null)
                    this.ns.Dispose();
            }
            finally
            {
                this.ns = null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        #endregion

    }
}
