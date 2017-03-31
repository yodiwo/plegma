using System;
using System.Collections.Generic;
using System.Linq;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double MoveTowards(this double value, double target, double byvalue)
        {
            if (value < target)
                return (value + byvalue).ClampCeil(target);
            else
                return (value - byvalue).ClampFloor(target);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double Lerp(this double value, double min, double max, double window_rescaler = 1f)
        {
            return min + (max - min) * window_rescaler * value.Saturate();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double Saturate(this double value)
        {
            return (value < 0) ? 0 : ((value > 1) ? 1 : value);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double Clamp(this double value, double min, double max)
        {
            return (value < min) ? min : ((value > max) ? max : value);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double InvertedClamp(this double value, double min, double max)
        {
            if (value > min && value < max)
            {
                var dist2min = value - min;
                var dist2max = max - value;
                return dist2min < dist2max ? min : max;
            }
            else
                return value;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double ClampFloor(this double value, double value2)
        {
            return (value > value2) ? value : value2;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double ClampCeil(this double value, double value2)
        {
            return (value < value2) ? value : value2;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double Rescale(this double value, double from_min, double from_max, double to_min, double to_max)
        {
            //clamp value
            double temp = value.Clamp(from_min, from_max);

            //offset to zero
            temp = temp - from_min;

            //rescale to 0..1
            temp = temp / (from_max - from_min);

            //rescale to new length
            temp = temp * (to_max - to_min);

            //offset to new min
            temp = temp + to_min;

            return temp;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool AlmostEqual(this double value, double testvalue, double error)
        {
            return (value > testvalue - error && value < testvalue + error);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool AlmostEqual(this double value, double testvalue, double lower_error, double upper_error)
        {
            return (value > testvalue - lower_error && value < testvalue + upper_error);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static double Abs(this double value)
        {
            return (value < 0 ? -value : value);
        }
    }
}
