using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Yodiwo.Media.Video.Sink
{
    public class ImageFile : IVideoSink
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public string filename;
        public bool IsActive { get; private set; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        public ImageFile(string fname)
        {
            this.filename = fname;
        }
        //------------------------------------------------------------------------------------------------------------------------
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
                bmp.Save(this.filename, System.Drawing.Imaging.ImageFormat.Jpeg);
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
                    try
                    {
                        image.Save(this.filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch { }
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
