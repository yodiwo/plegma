using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public interface IThingType
    {
        IEnumerable<ThingType> All();
        ThingType Get(string id);
    }
}
