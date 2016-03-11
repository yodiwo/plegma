using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Model
{
    [Serializable]
    public class GroupItem : Item
    {
        public List<Item> members { get; set; }

        public GroupItem()
        {
            this.members = new List<Item>();
        }
    }
}
