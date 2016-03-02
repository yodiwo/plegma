using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using NAudio.Wave;
using System.Threading;
using Yodiwo;
using Yodiwo.Media.Audio;

namespace Yodiwo.Media.Audio.Source.MP3Stream
{
    /// <summary>
    /// Provides a mp3streaming utility that can be used to stream audio from a public url
    /// </summary>
    public class MP3Stream : IAudioSource
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        private StreamingPlaybackState playbackState;
        private PipeStream pipe;
        private string mp3url;
        public event OnAudioDataEventHandler OnAudioDataCaptured;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        public AudioFormat audioFormat { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        enum StreamingPlaybackState
        {
            Stopped,
            Playing,
            Buffering,
            Paused
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Action<MP3Stream, WaveFormat> OnPCMStart = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructor
        public MP3Stream(string url)
        {
            this.mp3url = url;
        }
        #endregion


        #region Functions

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Provides a pcm stream that can be used to a waveout device (i.e. speakers) 
        /// or can write pcm samples to a System.IO.Stream
        /// </summary>
        public void Start()
        {
            //declares
            IMp3FrameDecompressor decompressor = null;
            HttpWebResponse resp = null;
            IsActive = true;

            //init state to buffering
            playbackState = StreamingPlaybackState.Buffering;

            //do web request
            var webRequest = (HttpWebRequest)WebRequest.Create(this.mp3url);
            try
            {
                resp = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex);
            }
            if (resp == null)
                return;

            //local buffer that is overriden each  time a mp3 frame is decompressed
            var buffer = new byte[16384 * 4];
            var respStream = resp.GetResponseStream();
            var readFullyStream = new ReadFullyStream(respStream);

            //Streaming Loop
            try
            {
                while (playbackState != StreamingPlaybackState.Stopped)
                {
                    //get frame
                    Mp3Frame frame;
                    try
                    {
                        frame = Mp3Frame.LoadFromStream(readFullyStream);
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceError(ex.Message);
                        break;
                    }

                    //get the codec info from the first frame
                    if (decompressor == null)
                    {
                        decompressor = Tools.CreateFrameDecompressor(frame);
                        audioFormat = new AudioFormat(decompressor.OutputFormat.SampleRate, (ushort)decompressor.OutputFormat.BitsPerSample, (ushort)decompressor.OutputFormat.Channels);
                        if (OnPCMStart != null)
                            OnPCMStart(this, decompressor.OutputFormat);
                    }

                    //write decompressed (pcm) samples to local buffer
                    int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                    if (IsActive)
                    {
                        //fire event
                        if (OnAudioDataCaptured != null)
                            OnAudioDataCaptured(this, new AudioEventArgs(buffer, decompressed));
                    }
                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex.Message);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Pause()
        {
            lock (this)
            {
                IsActive = false;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void Resume()
        {
            lock (this)
            {
                IsActive = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        public void Stop()
        {
            lock (this)
            {
                IsActive = false;
                playbackState = StreamingPlaybackState.Stopped;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------


        #endregion
    }


}



