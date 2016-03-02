using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Media
{
    public interface IMediaSS
    {
        bool IsActive { get; }
        void Start();
        void Stop();
    }
}
