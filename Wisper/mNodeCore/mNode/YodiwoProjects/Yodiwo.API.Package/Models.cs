using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.API.Packages.Models
{
    [Serializable]
    public class PackagesQueryResults
    {
        /// <summary> Packages manifests </summary>
        public List<string> Packages;
    }

    [Serializable]
    public class PackagesUpdateCheckRequest
    {
        /// <summary> A list of puids to get latest versions </summary>
        public List<string> PUID;
    }

    [Serializable]
    public class PackagesUpdateCheckResponse
    {
        /// <summary> A dictionary or puids(key) and their respective version (value) </summary>
        public Dictionary<string, int> Versions;
    }
}
