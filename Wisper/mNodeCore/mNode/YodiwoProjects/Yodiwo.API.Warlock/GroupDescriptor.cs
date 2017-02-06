using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public class GroupDescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public GroupKey GroupKey;
        public string Name;
        public string Icon;
        public string Type;
        public List<ThingDescriptor> Things;
        public HashSetTS<string> Tags;
        public int ThingsCount;
        public bool AutoUpdate;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public GroupDescriptor() { }

        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
