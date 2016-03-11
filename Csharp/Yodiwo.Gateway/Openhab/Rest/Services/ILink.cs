using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public interface ILink
    {
        IEnumerable<Link> All();
        bool Link(string name, string channel);
        bool UnLink(string name, string channel);
    }
}
