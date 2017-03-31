using System;
using System.Collections.Generic;
using System.Linq;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static int ClampFloor(this int value, int min)
        {
            return value > min ? value : min;
        }

        public static int ClampCeil(this int value, int max)
        {
            return value < max ? value : max;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool isBetweenValues(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static bool isBetweenValues_NonInclusive(this int value, int min, int max)
        {
            return value > min && value < max;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool AlmostEqual(this int value, int test_value, int error)
        {
            return value >= test_value - error && value <= test_value + error;
        }

        public static bool AlmostEqual(this int value, int test_value, int lower_error, int higher_error)
        {
            return value >= test_value - lower_error && value <= test_value + higher_error;
        }

        public static bool AlmostEqual_NonInclusive(this int value, int test_value, int error)
        {
            return value > test_value - error && value < test_value + error;
        }

        public static bool AlmostEqual_NonInclusive(this int value, int test_value, int lower_error, int higher_error)
        {
            return value > test_value - lower_error && value < test_value + higher_error;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static int Clamp(this int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static char ToChar(this int value)
        {
            return Convert.ToChar(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static int Abs(this int value)
        {
            return (value < 0 ? -value : value);
        }
    }
}
