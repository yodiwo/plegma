using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace Yodiwo.Media.Audio.Sink
{
    public class Speaker : IAudioSink
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        WaveOut waveout;
        VolumeWaveProvider16 volumeProvider;
        BufferedWaveProvider bufferedWaveProvider;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public Speaker()
        {
            waveout = new WaveOut();
            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 2));
            waveout.PlaybackStopped += Waveout_PlaybackStopped;
            volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
            waveout.Init(volumeProvider);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        private void Waveout_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            DebugEx.TraceLog("Playback Stopped");
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            waveout.Play();
            IsActive = true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            waveout.Stop();
            IsActive = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AddPCMdata(byte[] pcm)
        {
            bufferedWaveProvider.AddSamples(pcm, 0, pcm.Length);
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
