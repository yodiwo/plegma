using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Yodiwo.Media.Video.Sink
{
    public class ImageFileSequence : IVideoSink
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public string foldername;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsActive { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        public int FrameIndex = 0;
        public string FileFormat = "Frame_{n}.jpg";
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        public ImageFileSequence(string foldername)
        {
            this.foldername = foldername;
        }
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            IsActive = true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            IsActive = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AddFrame(Image bmp)
        {
            try
            {
                FrameIndex++;
                bmp.Save(Path.Combine(this.foldername, FileFormat.Replace("{n}", FrameIndex.ToString())), System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch { }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void AddFrame(byte[] imgdata)
        {
            try
            {
                using (var memStream = new MemoryStream(imgdata))
                using (var image = Image.FromStream(memStream))
                {
                    AddFrame(image);
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Could not create image from stream"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Flush()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Clear()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
