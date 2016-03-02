using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Media.Audio.Sink
{
    public interface IAudioSink : IMediaSink
    {
        void AddPCMdata(byte[] pcm);
    }
}
