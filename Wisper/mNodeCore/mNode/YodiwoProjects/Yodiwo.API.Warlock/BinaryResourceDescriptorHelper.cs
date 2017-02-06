using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public static class BinaryResourceDescriptorHelper
    {
        public static eBinaryResourceContentType HttpContentTypeToEnum(string HttpContentType)
        {
            if (HttpContentType.IsNullOrEmpty())
                return eBinaryResourceContentType.Undefined;
            else if (HttpContentType.Contains("multipart") || HttpContentType.Contains("application"))
                return eBinaryResourceContentType.Data;
            else if (HttpContentType.Contains("text"))
                return eBinaryResourceContentType.Text;
            else if (HttpContentType.Contains("image"))
                return eBinaryResourceContentType.Image;
            else if (HttpContentType.Contains("audio"))
                return eBinaryResourceContentType.Audio;
            else if (HttpContentType.Contains("video"))
                return eBinaryResourceContentType.Video;
            else
                return eBinaryResourceContentType.Undefined;
        }
    }
}
