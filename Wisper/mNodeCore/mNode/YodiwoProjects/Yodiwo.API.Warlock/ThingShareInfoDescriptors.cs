using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{

    public class ThingShareInfoDescriptor
    {
        public string Name;
        //-------------------------------------------------------------------------------------------------------------------------
        public string Email;      // Webuser's e-mail
        public string OwnerEmail; // user e-mail that owns this Thing
        public bool Owned;
        public IEnumerable<string> SharedEmails;
        //-------------------------------------------------------------------------------------------------------------------------
        public ThingKey ThingKey;
        public int PortsLength;
        public string Type;
        public bool Removable;
    }

}
