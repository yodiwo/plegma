using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static class MathTools
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public readonly static Random objRandom = new Random(((int)((System.DateTime.Now.Ticks % System.Int32.MaxValue))));

        //----------------------------------------------------------------------------------------------------------------------------------------------
        public const float PI = (float)Math.PI;
        public const float halfPI = (float)(Math.PI / 2d);

        public const double RadiansToDegrees = 180 / Math.PI;
        public const double DegreesToRadians = Math.PI / 180;

        public const float RadiansToDegreesF = (float)RadiansToDegrees;
        public const float DegreesToRadiansF = (float)DegreesToRadians;

        //----------------------------------------------------------------------------------------------------------------------------------------------

        //** DO NOT CHANGE THE VALUES OF THIS ARRAYS AT RUNTIME. **
        //You can use this arrays for separators in String.Split(..., StringSplitOptions.RemoveEmptyEntries);
        //and avoid creating new arrays/object etc

        public static readonly char[] Separators_Dash = new char[] { '-' };
        public static readonly char[] Separators_Dot = new char[] { '.' };
        public static readonly char[] Separators_BackSlash = new char[] { '\\' };
        public static readonly char[] Separators_ForwardSlash = new char[] { '/' };
        public static readonly char[] Separators_Plus = new char[] { '+' };
        public static readonly char[] Separators_Sharp = new char[] { '#' };
        public static readonly char[] Separators_At = new char[] { '@' };
        public static readonly char[] Separators_Comma = new char[] { ',' };
        public static readonly char[] Separators_Equals = new char[] { '=' };
        public static readonly char[] Separators_Ampersand = new char[] { '&' };

        //----------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compute the smoothing lerp factor based on delta ..
        /// A smoothing value of 0.5 means y=log2(1/2^x) ,so the oldvalue will reach :
        /// 99.9% of the newvalue in 3 frames = 0.005 sec = 5 ms
        /// 99.99% of the newvalue in 6 frames = 0.01 sec = 10 ms
        /// must be Lerp( oldvale , newvale , LerpSmoothing(..) );
        /// </summary>
        public static float LerpSmoothing(float Smoothing, float NormalizedDelta)
        {
            return (float)(1f - Math.Pow(Smoothing.Clamp(0, 1f), NormalizedDelta)).Saturate();
        }

        public static float LerpSmoothing(float Smoothing, double NormalizedDelta)
        {
            return (float)(1f - Math.Pow(Smoothing.Clamp(0, 1f), NormalizedDelta)).Saturate();
        }

        public static double LerpSmoothingD(float Smoothing, double NormalizedDelta)
        {
            return (1f - Math.Pow(Smoothing.Clamp(0, 1f), NormalizedDelta)).Saturate();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static int GetRandomNumber<T>(T[] array)
        {
            return GetRandomNumber(0, array.Length);
        }

        public static int GetRandomNumber()
        {
            lock (objRandom)
                return objRandom.Next();
        }

        public static int GetRandomNumber(int Low, int High)
        {
            // Returns a random number,
            // between the optional Low and High parameters
            lock (objRandom)
                return objRandom.Next(Low, (High + 1));
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float GetRandomNumber(float Low, float High)
        {
            return Low + GetRandomNumber_Normalized() * (High - Low);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool GetRandomBoolean()
        {
            lock (objRandom)
                return objRandom.Next(0, 2) == 0;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T GetRandomItem<T>(T[] source)
        {
            int index;
            return GetRandomItem<T>(source, out index);
        }

        public static T GetRandomItem<T>(IList<T> source)
        {
            int index;
            return GetRandomItem<T>(source, out index);
        }

        public static T GetRandomItem<T>(IReadOnlyList<T> source)
        {
            int index;
            return GetRandomItem<T>(source, out index);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T GetRandomItem<T>(IList<T> source, out int Index)
        {
            Index = -1;
            DebugEx.Assert(source != null, "Null source detected");
            DebugEx.Assert(source != null && source.Count > 0, "Source has no elements");
            if (source == null || source.Count == 0)
                return default(T);
            else
            {
                lock (objRandom)
                    Index = objRandom.Next(0, (source.Count - 1 + 1));
                return source[Index];
            }
        }

        public static T GetRandomItem<T>(IReadOnlyList<T> source, out int Index)
        {
            Index = -1;
            DebugEx.Assert(source != null, "Null source detected");
            DebugEx.Assert(source != null && source.Count > 0, "Source has no elements");
            if (source == null || source.Count == 0)
                return default(T);
            else
            {
                lock (objRandom)
                    Index = objRandom.Next(0, (source.Count - 1 + 1));
                return source[Index];
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T GetRandomItem<T>(T[] source, out int Index)
        {
            Index = -1;
            DebugEx.Assert(source != null, "Null source detected");
            DebugEx.Assert(source != null && source.Length > 0, "Source has no elements");
            if (source == null || source.Length == 0)
                return default(T);
            else
            {
                lock (objRandom)
                    Index = objRandom.Next(0, (source.Length - 1 + 1));
                return source[Index];
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static T GetRandomItem<T>(T[] source, out int Index, Func<int, bool> filter = null, int MaxRetries = 10)
        {
            Index = -1;
            DebugEx.Assert(source != null, "Null source detected");
            DebugEx.Assert(source != null && source.Length > 0, "Source has no elements");
            if (source == null || source.Length == 0)
                return default(T);
            else
                lock (objRandom)
                {
                    lock (objRandom)
                        Index = objRandom.Next(0, (source.Length - 1 + 1));
                    //filter and retry
                    if (filter != null)
                        while (!filter(Index))
                        {
                            if (MaxRetries <= 0)
                            {
                                Index = -1;
                                return default(T);
                            }
                            MaxRetries--;
                            lock (objRandom)
                                Index = objRandom.Next(0, (source.Length - 1 + 1));
                        }
                    return source[Index];
                }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static UInt64 GetRandomNumber64()
        {
            lock (objRandom)
            {
                // Returns a random number in the uin64bit range
                var val1 = (UInt64)objRandom.Next(0, int.MaxValue);
                var val2 = (UInt64)objRandom.Next(0, int.MaxValue);
                var val3 = (UInt64)objRandom.Next(0, int.MaxValue);
                return ((val1 << 33) | val2) ^ (val3 << 16);
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float GetRandomNumber_Normalized()
        {
            lock (objRandom)
            {
                //returns a number from 0..1
                int max = System.Int32.MaxValue;
                return (float)(objRandom.Next(0, max) / (float)max);
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float GetRandomNumber_MinusOne2One()
        {
            lock (objRandom)
            {
                //returns a number from -1..1
                int max = System.Int32.MaxValue;
                return ((float)(objRandom.Next(0, max) / (float)max) - 0.5f) * 2;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static int PointsToPixels(int points, int dpi = 96)
        {
            return points * 96 / 72;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static void Swap<T>(ref T v1, ref T v2)
        {
            T tmp;
            tmp = v1;
            v1 = v2;
            v2 = tmp;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static float ComputeKineticEnegry(float Velocity, float Mass)
        {
            return 0.5f * Velocity * Velocity * Mass;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static readonly ReadOnlyList<char> numbers = new ReadOnlyList<char>(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
        public static readonly ReadOnlyList<char> alphabet = new ReadOnlyList<char>(new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' });
        public static readonly ReadOnlyList<char> alphabetLower = new ReadOnlyList<char>(alphabet.Select(c => Char.ToLowerInvariant(c)).ToArray());

        public static readonly ReadOnlyList<char> alphaNumeric = new ReadOnlyList<char>(alphabet.Concat(numbers).ToArray());
        public static readonly ReadOnlyList<char> alphaNumericlow = new ReadOnlyList<char>(alphaNumeric.Concat(alphabetLower).ToArray());

        public static string GenerateRandomAlphaNumericString(int length, bool useLowCase = true)
        {
            return GenerateRandomString(length, useLowCase ? alphaNumericlow : alphaNumeric);
        }

        public static string GenerateRandomString(int length, char[] charset)
        {
            var strbld = new StringBuilder(length);
            for (int n = 0; n < length; n++)
                strbld.Append(GetRandomItem(charset));
            return strbld.ToString();
        }

        public static string GenerateRandomString(int length, IReadOnlyList<char> charset)
        {
            var strbld = new StringBuilder(length);
            for (int n = 0; n < length; n++)
                strbld.Append(GetRandomItem(charset));
            return strbld.ToString();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
