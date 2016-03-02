using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pranas;
using System.Drawing;

namespace Yodiwo.Media.Video.Source
{
    public class ScreenFrameTrapper : IVideoSource
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public event VideoFramesEventHandler OnFrameCaptured;
        public bool IsActive { get; private set; }
        private bool isRunning = false;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            lock (this)
            {
                IsActive = true;
                isRunning = true;
            }
            Task.Run(() => Capture());
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Capture()
        {
            while (isRunning)
            {
                var frame = Pranas.ScreenshotCapture.TakeScreenshot();
                if (IsActive)
                    OnFrameCaptured(this, new VideoEventArgs(new Bitmap(frame)));
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            lock (this)
            {
                IsActive = false;
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
