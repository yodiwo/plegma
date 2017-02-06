using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Packages
{
    public static class FlavorTester
    {
        public static bool TestFlavors(PackagesAPI.PackageFlavors flavor, string value1, string value2)
        {
            if (flavor == PackagesAPI.PackageFlavors.Version)
            {
                //examine versions
                return RangedIntegerMatch(value1, value2);
            }
            else if (flavor == PackagesAPI.PackageFlavors.Platform)
            {
                //examine platforms
                return value1 == value2;
            }
            else if (flavor == PackagesAPI.PackageFlavors.Architecture)
            {
                //examine architectures
                return value1 == value2;
            }
            else if (flavor == PackagesAPI.PackageFlavors.Unkown)
            {
                DebugEx.Assert("Unkown flavor detected");
                return true;
            }
            else
            {
                DebugEx.Assert("Unkown flavor " + flavor);
                return value1 == value2;    //simple string match in unkown cases
            }
        }


        static bool RangedIntegerMatch(string value1, string value2)
        {
            try
            {
                Int64 range1Min, range1Max;
                Int64 range2Min, range2Max;

                //find ranges for value1
                if (value1.IndexOf(PackagesAPI.PackageFlavorRangeSeparator) == -1)
                    range1Min = range1Max = value1.ParseToInt64();
                else
                {
                    var splits = value1.Split(PackagesAPI.PackageFlavorRangeSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
                    var first = splits[0].ParseToInt64();
                    var second = splits[1].ParseToInt64();
                    range1Min = Math.Min(first, second);
                    range1Max = Math.Max(first, second);
                }

                //find ranges for value2
                if (value2.IndexOf(PackagesAPI.PackageFlavorRangeSeparator) == -1)
                    range2Min = range2Max = value2.ParseToInt64();
                else
                {
                    var splits = value2.Split(PackagesAPI.PackageFlavorRangeSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
                    var first = splits[0].ParseToInt64();
                    var second = splits[1].ParseToInt64();
                    range2Min = Math.Min(first, second);
                    range2Max = Math.Max(first, second);
                }

                //examine range intersections
                return (range2Min >= range1Min && range2Min <= range1Max) ||
                       (range2Max >= range1Min && range2Max <= range1Max);
            }
            catch { return false; }
        }
    }
}
