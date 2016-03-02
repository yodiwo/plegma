using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Yodiwo.Media.Audio.Source
{
    public class Microphone : IAudioSource
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public WaveInEvent waveSource = null;
        public event OnAudioDataEventHandler OnAudioDataCaptured;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        public AudioFormat audioFormat { get; private set; }

        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            audioFormat = new AudioFormat(8000, 16, 2);
            IsActive = true;
            waveSource = new WaveInEvent();
            //wave format
            waveSource.WaveFormat = new WaveFormat(audioFormat.samplerate, audioFormat.bitsperchannel, audioFormat.channels);
            //register event cbs
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
            //start record from mic
            waveSource.StartRecording();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            waveSource.StopRecording();
        }
        //------------------------------------------------------------------------------------------------------------------------
        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (IsActive)
                if (OnAudioDataCaptured != null)
                    OnAudioDataCaptured(this, new Yodiwo.Media.Audio.Source.AudioEventArgs(e.Buffer, e.Buffer.Length));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Pause()
        {
            lock (this)
                IsActive = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            lock (this)
            {
                if (waveSource != null)
                {
                    waveSource.Dispose();
                    waveSource = null;
                    IsActive = false;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Resume()
        {
            lock (this)
                IsActive = true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
