using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Media
{
    public interface IMediaSink : IMediaSS
    {
        void Flush();
        void Clear();
    }
}
