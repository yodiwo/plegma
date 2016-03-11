using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public class HABService
    {
        public IItem Items { get; set; }
        public IBinding Bindings { get; set; }
        public IDiscovery Discovery { get; set; }
        public IThingType ThingTypes { get; set; }
        public ILink Links { get; set; }
        public IInbox Inbox { get; set; }
        public IThing Things { get; set; }
        public ISetup Setup { get; set; }

        public HABService(string baseurl)
        {
            Items = new ItemService(baseurl);
            Bindings = new BindingService(baseurl);
            Discovery = new DiscoveryService(baseurl);
            ThingTypes = new ThingTypeService(baseurl);
            Links = new LinkService(baseurl);
            Inbox = new InboxService(baseurl);
            Things = new ThingService(baseurl);
            Setup = new SetupService(baseurl);
        }
    }
}
