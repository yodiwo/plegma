using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;

namespace Yodiwo.Media.Audio.Sink
{
    public class AudioPipeStream : IAudioSink
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        public PipeStream Pipe;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public AudioPipeStream()
        {
            Pipe = new PipeStream();
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            lock (this)
            {
                //write header
                Tools.WriteHeader(Pipe, int.MaxValue, 8000, 16, 2);
                //mark active
                IsActive = true;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            lock (this)
            {
                if (!IsActive)
                    return;

                //close stream
                try { Pipe.Close(); } catch { }

                //mark stopped
                IsActive = false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AddPCMdata(byte[] pcmdata)
        {
            lock (this)
            {
                if (!IsActive)
                    return;

                //write to stream
                try { Pipe.Write(pcmdata, 0, pcmdata.Length); } catch { }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Flush()
        { }
        //------------------------------------------------------------------------------------------------------------------------

        public void Clear()
        { }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
