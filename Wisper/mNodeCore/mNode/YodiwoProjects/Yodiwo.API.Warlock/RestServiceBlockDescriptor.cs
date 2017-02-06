using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public class RestServiceBlockDescriptor
    {
        //TODO: make a proper representation
        public string Name;
        public string IconUrl;

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public RestServiceBlockDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public RestServiceBlockDescriptor(string BlockName, string Icon)
        {
            this.Name = BlockName;
            this.IconUrl = Icon;
        }

        #endregion
    }

}
