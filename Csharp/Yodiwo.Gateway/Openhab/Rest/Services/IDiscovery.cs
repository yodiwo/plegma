using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public interface IDiscovery
    {
        IEnumerable<string> All();
        bool Scan(string id);
    }
}
