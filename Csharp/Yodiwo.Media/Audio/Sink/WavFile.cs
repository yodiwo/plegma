using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Yodiwo.Media.Audio.Sink
{
    public class WavFile : IAudioSink
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //MemoryStream ms;
        string filename;
        FileStream fs;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public WavFile(string filename)
        {
            this.filename = filename;
            fs = new FileStream(this.filename, FileMode.Create, FileAccess.Write, FileShare.Read);
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
                Tools.WriteHeader(fs, int.MaxValue, 8000, 16, 2);
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
                try { fs.Close(); } catch { }

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
                try { fs.Write(pcmdata, 0, pcmdata.Length); } catch { }
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
