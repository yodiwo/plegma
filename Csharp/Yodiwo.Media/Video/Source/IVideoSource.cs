using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Yodiwo.Media.Video.Source
{
    public delegate void VideoFramesEventHandler(object sender, VideoEventArgs videodata);
    public interface IVideoSource : IMediaSource
    {
        //void OnFrameCaptured(Bitmap bmp);
        event VideoFramesEventHandler OnFrameCaptured;
    }

    public class VideoEventArgs : EventArgs
    {
        public Bitmap bitmap;

        public VideoEventArgs(Bitmap bmp)
        {
            this.bitmap = bmp;
        }
    }
}
