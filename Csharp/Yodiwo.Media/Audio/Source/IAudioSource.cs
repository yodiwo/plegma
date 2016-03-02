using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Media.Audio.Source
{
    public delegate void OnAudioDataEventHandler(object sender, AudioEventArgs audiodata);
    public interface IAudioSource : IMediaSource
    {
        AudioFormat audioFormat { get; }

        event OnAudioDataEventHandler OnAudioDataCaptured;
    }

    public class AudioEventArgs
    {
        public byte[] pcmdata;
        public int length;

        public AudioEventArgs(byte[] audiodata, int audiolen)
        {
            this.pcmdata = audiodata;
            this.length = audiolen;
        }
    }
}
