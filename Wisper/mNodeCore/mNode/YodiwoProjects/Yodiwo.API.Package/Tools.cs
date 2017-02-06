using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yodiwo;

namespace Yodiwo.API.Packages
{
    public static class Tools
    {
        //------------------------------------------------------------------------------------------------------------------------
        public static string[] GetFlavorlessPUIDComponents(string puid)
        {
            return puid.Split(PackagesAPI.PackageChainSeparator).Select(pc => GetFlavorlessComponent(pc)).ToArray();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string GetFlavorlessPUID(string puid)
        {
            DebugEx.Assert("TODO"); //TODO: GetFlavorlessPUID
            return "";
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string GetFlavorlessComponent(string Component)
        {
            return Component.LeftOf(PackagesAPI.PackageFlavorSeparator);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string GetFlavorlessComponentFromPUID(string puid)
        {
            if (string.IsNullOrWhiteSpace(puid))
                return puid;
            else if (puid.IndexOf(PackagesAPI.PackageChainSeparator) == -1)
                return GetFlavorlessComponent(puid);
            else
                return GetFlavorlessComponent(puid.Split(PackagesAPI.PackageChainSeparator).Last());
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string GetFlavoredComponentFromPUID(string puid)
        {
            if (string.IsNullOrWhiteSpace(puid))
                return puid;
            else if (puid.IndexOf(PackagesAPI.PackageChainSeparator) == -1)
                return puid;
            else
                return puid.Split(PackagesAPI.PackageChainSeparator).Last();
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string AppendPackageFlavorValue(string Component, PackagesAPI.PackageFlavors flavor, string value)
        {
            if (Validators.ValidatePackageFlavor(value))
                DebugEx.Assert("Invalid flavor value");

            return Component + PackagesAPI.PackageFlavorSeparator + PackagesAPI.PackageFlavorKey[flavor] + value;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string[] GetPackageRawFlavors(string Component)
        {
            if (Component.IndexOf(PackagesAPI.PackageFlavorSeparator) == -1)
                return null;

            //split
            return Component.RightOf(PackagesAPI.PackageFlavorSeparator).Split(PackagesAPI.PackageFlavorSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<PackagesAPI.PackageFlavors, string> GetPackageFlavors(string Component, bool toLower = true)
        {
            var res = new Dictionary<PackagesAPI.PackageFlavors, string>();

            //no flavors
            if (Component.IndexOf(PackagesAPI.PackageFlavorSeparator) == -1)
                return res;

            //get raw flavors
            var splits = GetPackageRawFlavors(Component);

            //process raw flavors kv
            for (int n = 0; n < splits.Length; n++)
            {
                //find flavor
                foreach (var f in PackagesAPI.PackageFlavorOrder)
                {
                    var fkey = PackagesAPI.PackageFlavorKey[f];
                    if (splits[n].StartsWith(fkey, false))
                    {
                        try
                        {
                            var flavorValue = splits[n].Substring(fkey.Length);
                            if (toLower) flavorValue = flavorValue.ToLowerInvariant();
                            res.Add(f, flavorValue);
                        }
                        catch { }
                        break;
                    }
                }
            }

            return res;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static string GetPackageFlavorValue(string Component, PackagesAPI.PackageFlavors flavor, string Default = "")
        {
            if (Component.IndexOf(PackagesAPI.PackageFlavorSeparator) == -1)
                return Default;

            //split
            var splits = GetPackageRawFlavors(Component);
            if (splits.Length <= 0)
                return Default;

            //find flavor
            var flavorkey = PackagesAPI.PackageFlavorKey[flavor];
            for (int n = 0; n < splits.Length; n++)
            {
                if (splits[n].StartsWith(flavorkey, false))
                    return splits[n].Substring(flavorkey.Length);
            }

            return Default;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static bool PackageFlavorMatch(string Component1, string Component2)
        {
            if (Component1 == Component2)
                return true;
            if (Component1 == null || Component2 == null)
                return false;

            //get flavors
            var flavors1 = GetPackageFlavors(Component1.ToLowerInvariant());
            var flavors2 = GetPackageFlavors(Component2.ToLowerInvariant());
            if (flavors1.Count <= 0 || flavors2.Count <= 0)
                return true;

            //examine flavors
            foreach (var fkv1 in flavors1)
                if (flavors2.ContainsKey(fkv1.Key))
                {
                    //check if flavors match.. if not the stop and return false since packages are incompatible
                    if (!FlavorTester.TestFlavors(fkv1.Key, fkv1.Value, flavors2[fkv1.Key]))
                        return false;
                }

            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
    }
}
