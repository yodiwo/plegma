using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API
{
    /// <summary>
    /// Base class of an API message, from which all message classes inherit
    /// </summary>
    [Newtonsoft.Json.JsonSafeType()]
    [Serializable]
    public abstract class ApiMsg
    {
        /// <summary>
        /// Sequence number of this message
        /// </summary>
        public int SeqNo;
    }
}
