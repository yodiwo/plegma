using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.API.Packages
{
    public static class PackagesAPI
    {
        public const char PackageChainSeparator = '.';
        public const string PackageChainSeparatorStr = ".";
        public static readonly char[] PackageChainSeparatorArray = new[] { PackageChainSeparator };

        public const char PackageFlavorSeparator = '-';
        public const string PackageFlavorSeparatorStr = "-";
        public static readonly char[] PackageFlavorSeparatorArray = new[] { PackageFlavorSeparator };

        public const char PackageFlavorRangeSeparator = ':';
        public static readonly char[] PackageFlavorRangeSeparatorArray = new[] { ':' };

        public const string PackageServerURI = "Packages";
        public const string PackageServer_List = "List";
        public const string PackageServerURI_List = PackageServerURI + "/" + PackageServer_List;
        public const string PackageServer_GetPackage = "GetPackage";
        public const string PackageServerURI_GetPackage = PackageServerURI + "/" + PackageServer_GetPackage;
        public const string PackageServer_UpdateCheck = "UpdateCheck";
        public const string PackageServerURI_Updates = PackageServerURI + "/" + PackageServer_UpdateCheck;

        public enum PackageType : int
        {
            Unkown = 0,
            Archive,
            Debian,
            WinInstaller,
            Msi,
            UWPApp,

            Extension,
        }

        public enum PackageFlavors : int
        {
            Unkown = 0,
            Architecture,
            Platform,
            Version,
        }
        public static IReadOnlyDictionary<PackageFlavors, string> PackageFlavorKey = new Dictionary<PackageFlavors, string>()
        {
            { PackageFlavors.Architecture, "a"},
            { PackageFlavors.Platform, "p"},
            { PackageFlavors.Version,"v"},
        };
        public static IReadOnlyDictionary<string, PackageFlavors> PackageFlavorKeyReverse = PackageFlavorKey.Select(kv => new KeyValuePair<string, PackageFlavors>(kv.Value, kv.Key)).ToDictionary();

        public static PackageFlavors[] PackageFlavorOrder = new PackageFlavors[]
        {
            PackageFlavors.Version,
            PackageFlavors.Platform,
            PackageFlavors.Architecture,
        };
    }
}
