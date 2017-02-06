using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Warlock;

namespace Yodiwo.API.Warlock
{
    /// <summary>
    /// auxiliary class for OAuth stuff
    /// </summary>
    public class OAuth
    {

        #region Variables

        /// <summary>
        /// all available oauth scopes
        /// </summary>
        public static readonly List<string> Scopes = new List<string>
        {
            "default"
        };

        public static readonly DictionaryTS<string, OAuthClientInfo> Clients = new DictionaryTS<string, OAuthClientInfo>
        {
            {"asdf", new OAuthClientInfo("asdf", "http://localhost:4321/placeholder")},
            {"zxcv", new OAuthClientInfo("zxcv", null) },
        };

        /// <summary>
        /// scope separator
        /// </summary>
        public const char ScopeSeparator = ';';

        //-------------------------------------------------------------------------------------------------------------------------

        #endregion

        #region Constructors

        #endregion

        #region Functions

        #endregion

    }

    public class OAuthClientInfo
    {
        public string ClientId;
        public string RedirectUrl;

        public OAuthClientInfo()
        { }

        public OAuthClientInfo(string clientId, string redirectUrl)
        {
            ClientId = clientId;
            RedirectUrl = redirectUrl;
        }
    }
}


