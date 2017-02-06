using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    [GeneratorIgnore]
    public static class Validators
    {
        static readonly Regex KeyValidatorExpression = new Regex(@"^[A-Za-z0-9/#\$\[\]@%_.:|\-]+$");

        /// <summary> Validates key format type ( alphanumeric+dashes ) </summary>
        public static bool ValidateKey(string Key)
        {
            return string.IsNullOrWhiteSpace(Key) ? false : KeyValidatorExpression.IsMatch(Key);
        }

    }
}
