using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public enum Endianness
    {
        Unkown = 0,
        LittleEndian = 1,
        BigEndian = 2,
    }

    public enum HttpMethods
    {
        Get,
        Post,
        Put,
        //Options,
    }

    public enum HttpRequestDataFormat
    {
        Json,
        FormData,
        Text,
        Xml,
    }


    public struct SimpleActionResult
    {
        public bool IsSuccessful;
        public string Message;

        public static implicit operator Boolean(SimpleActionResult Result) { return Result.IsSuccessful; }
    }
}
