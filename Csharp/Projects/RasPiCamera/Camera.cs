using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Media;
using Yodiwo.Media.Video.Source;
using System.Drawing;

namespace Yodiwo.Projects.RasPiCamera
{
    public class Camera : IVideoSource
    {
        #region Variables
        public event VideoFramesEventHandler OnFrameCaptured;
        public bool IsActive { get; private set; }
        public Transport sharppytransport;
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public Camera(Transport transport)
        {
            this.sharppytransport = transport;
            this.sharppytransport.tcpimageserver.OnRasPiImagecb = OnNewFrame;
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
                var message = new SharpPy()
                {
                    operation = CMD.Start
                };
                DebugEx.TraceLog("Send to python Start Capture");
                this.sharppytransport.Send2python(message);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            lock (this)
            {
                IsActive = false;
                var msg = new SharpPy()
                {
                    operation = CMD.Stop
                };
                this.sharppytransport.Send2python(msg);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnNewFrame(Bitmap bmp)
        {
            if (IsActive)
                OnFrameCaptured(this, new VideoEventArgs(bmp));
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
