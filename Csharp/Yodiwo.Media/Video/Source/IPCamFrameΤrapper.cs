using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Drawing;
using AForge.Video.DirectShow;
using AForge.Video;

namespace Yodiwo.Media.Video.Source
{
    public class IPCamFrameΤrapper : IVideoSource
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        bool isRunning = false;
        string sourceURL;
        string uname;
        string pwd;
        bool securehttp;
        public event VideoFramesEventHandler OnFrameCaptured;
        public bool IsActive { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public IPCamFrameΤrapper(string uri, string username, string passwd)
        {
            this.sourceURL = uri;
            this.uname = username;
            this.pwd = passwd;
            if (this.sourceURL.StartsWith("https"))
                this.securehttp = true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            IsActive = true;
            this.isRunning = true;
            Task.Run(() => Capturing());

        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Capturing()
        {
            while (this.isRunning)
            {
                //accept certificate if ssl is enabled
                if (this.securehttp)
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

                // create HTTP request
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.sourceURL);
                req.Credentials = new NetworkCredential(this.uname, this.pwd);
                // get response
                WebResponse resp = req.GetResponse();
                // get response stream
                Stream stream = resp.GetResponseStream();
                // read data from stream
                byte[] buffer = new byte[100000];
                int read, total = 0;

                while ((read = stream.Read(buffer, total, 1000)) != 0)
                {
                    total += read;
                }
                // get bitmap
                MemoryStream ms = new MemoryStream(buffer, 0, total);
                ms.Seek(0, SeekOrigin.Begin);
                Bitmap bmp = new Bitmap(ms);
                OnFrameCapturedCb(bmp);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnFrameCapturedCb(Bitmap bitmap)
        {
            //handle videoframe
            if (IsActive)
                if (OnFrameCaptured != null)
                    OnFrameCaptured(this, new VideoEventArgs(bitmap));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            lock (this)
            {
                IsActive = false;
                this.isRunning = false;
            }
        }

        public void Pause()
        {
            lock (this)
                IsActive = false;
        }

        public void Resume()
        {
            lock (this)
                IsActive = true;

        }

        #endregion
    }


}
