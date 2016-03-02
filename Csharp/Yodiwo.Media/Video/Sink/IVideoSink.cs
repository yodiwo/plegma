using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Yodiwo.Media.Video.Sink
{
    public interface IVideoSink : IMediaSink
    {
        void AddFrame(Image bmp);
        void AddFrame(byte[] bytes);
    }
}
