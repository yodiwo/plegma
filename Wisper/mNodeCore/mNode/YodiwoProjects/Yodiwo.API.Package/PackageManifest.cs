using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.API.Packages
{
    [Serializable]
    public class PackageManifest
    {
        public string Name { get; set; }
        public int Version { get; set; }
        public string PUID { get; set; }
        public string Publisher { get; set; }
        public string PublisherURL { get; set; }
        public string PackageURL { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }

        public string IconUri { get; set; }
        public string ImageUri { get; set; }

        public Int64 FileSize { get; set; }

        public string[] Categories { get; set; }
        public string[] Tags { get; set; }
        public string[] Features { get; set; }
        public string[] Changes { get; set; }

        public string[] SupportedPlatforms { get; set; }
        public PackagesAPI.PackageType[] PackageTypes { get; set; }

        public string LicenseName { get; set; } //eg. MIT, Apache 2
        public string License { get; set; } //summary of the licence
        public string LicenseUri { get; set; } //the uri to the full license

        /// <summary> a list of puids the package depends on </summary>
        public string[] DependsOn { get; set; }

        /// <summary> the channels this package belongs to (used for filtering, default is "main") </summary>
        public string[] Channels { get; set; }
    }
}
