using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;

namespace Yodiwo.Media.Video.Source
{
    public class WebCamFrameTrapper : IVideoSource
    {
        #region Variables
        VideoCaptureDevice videoSource;
        public event VideoFramesEventHandler OnFrameCaptured;
        public bool IsActive { get; private set; }
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public WebCamFrameTrapper()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(OnNewFrame);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            lock (this)
            {
                IsActive = true;
                videoSource.Start();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;
            if (IsActive)
                OnFrameCaptured(this, new VideoEventArgs(bitmap));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            lock (this)
            {
                IsActive = false;
                videoSource.SignalToStop();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void Pause()
        {
            lock (this)
                IsActive = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Resume()
        {
            lock (this)
                IsActive = true;
        }
        #endregion
    }
}
