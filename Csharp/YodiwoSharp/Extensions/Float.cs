using System;
using System.Collections.Generic;
using System.Linq;


namespace Yodiwo
{
    public static partial class Extensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float MoveTowards(this float value, float target, float byvalue)
        {
            if (value < target)
                return (value + byvalue).ClampCeil(target);
            else
                return (value - byvalue).ClampFloor(target);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float Lerp(this float value, float min, float max, float window_rescaler = 1f)
        {
            return min + (max - min) * window_rescaler * value.Saturate();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float Saturate(this float value)
        {
            return (value < 0) ? 0 : ((value > 1) ? 1 : value);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float Clamp(this float value, float min, float max)
        {
            return (value < min) ? min : ((value > max) ? max : value);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float InvertedClamp(this float value, float min, float max)
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

        public static float ClampFloor(this float value, float value2)
        {
            return (value > value2) ? value : value2;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float ClampCeil(this float value, float value2)
        {
            return (value < value2) ? value : value2;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float Rescale(this float value, float from_min, float from_max, float to_min, float to_max)
        {
            //clamp value
            float temp = value.Clamp(from_min, from_max);

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

        public static bool AlmostEqual(this float value, float testvalue, float error)
        {
            return (value > testvalue - error && value < testvalue + error);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool AlmostEqual(this float value, float testvalue, float lower_error, float upper_error)
        {
            return (value > testvalue - lower_error && value < testvalue + upper_error);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float Abs(this float value)
        {
            return (value < 0 ? -value : value);
        }
    }
}
