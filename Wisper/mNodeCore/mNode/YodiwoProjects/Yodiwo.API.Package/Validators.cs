using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.API.Packages
{
    public class Validators
    {
        static readonly Regex PackagePUIDValidatorExpression = new Regex(@"^[A-Za-z0-9._:\-]+$");

        public static bool ValidatePackagePUID(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? false : PackagePUIDValidatorExpression.IsMatch(name);
        }


        static readonly Regex PackageFlavorValidatorExpression = new Regex(@"^[A-Za-z0-9_:]+$");

        public static bool ValidatePackageFlavor(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? false : PackageFlavorValidatorExpression.IsMatch(name);
        }

    }
}
