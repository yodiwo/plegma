using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public class ThingGroupDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string Name;
        public List<ThingDescriptor> Things;
        public List<string> Tags;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public ThingGroupDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
