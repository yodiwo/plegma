using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public class ApiKeyDescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string Name;
        public UserApiKey UserApiKey;
        public eApiKeyType Type;
        public string LinkedKey;
        public int TimesUsed;
        public string LastUsed;
        public string CreatedTimestamp;
        public bool IsEnabled;
        public QuotaDescriptor BytesIO;
        public QuotaDescriptor EventsIO;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public ApiKeyDescriptor() { }
        #endregion
    }

    public enum eApiKeyType
    {
        Rest = 0,
        Linked_To_Node = 1,
        BearerToken = 2,
    }
}
