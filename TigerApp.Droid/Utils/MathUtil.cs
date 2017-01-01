using System;
using Android.Graphics;

namespace TigerApp.Droid.Utils
{
    public static class MathUtil
    {
        public const double Pi = Math.PI;
        public const double DoublePi = 2 * Math.PI;
        public const float HalfPi = (float)(0.5f * Math.PI);
        public const double ToDegrees = 180 / Math.PI;
        public const double ToRadiansConst = Math.PI / 180;
        public const float PiF = (float)Math.PI;

        public static double GetAngle(Point vector1, Point vector2)
        {
            return Math.Atan2(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }

        public static double GetAngle(double x1, double y1, double x2, double y2)
        {
            return Math.Atan2(y1 - y2, x1 - x2);
        }

        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        public static bool IsOdd(this int value)
        {
            return value % 2 == 1;
        }

        public static int ClaimInt(this int value, int a, int b)
        {
            var min = a < b ? a : b;
            var max = a > b ? a : b;

            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static int ClaimInt(this float value, int a, int b)
        {
            var min = a < b ? a : b;
            var max = a > b ? a : b;

            if (value < min)
                return min;
            if (value > max)
                return max;

            return RoundToInt(value);
        }

        public static int ClaimInt(this double value, int a, int b)
        {
            return ClaimInt((float)value, a, b);
        }

        public static float ClaimFloat(this float value, float a, float b)
        {
            var min = a < b ? a : b;
            var max = a > b ? a : b;

            if (value < min)
                return min;
            if (value > max)
                return max;

            return value;
        }

        public static float ClaimFloat(this double value, float a, float b)
        {
            return ClaimFloat((float)value, a, b);
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static bool Between(this float value, float a, float b)
        {
            return value >= a && value <= b;
        }

        public static bool Between(this double value, double a, double b)
        {
            return value >= a && value <= b;
        }

        /**
		 * Calculates angle difference
		 * @param	angle1
		 * angle value in radians
		 * @param	angle2
		 * angle value in radians
		 * @return
		 * difference in range [-PI...PI]
		 */
        public static double AngleDiff(double angle1, double angle2)
        {
            angle1 %= DoublePi;
            angle2 %= DoublePi;

            if (angle1 < 0)
                angle1 += DoublePi;

            if (angle2 < 0)
                angle2 += DoublePi;

            var diff = angle2 - angle1;

            if (diff < -Pi)
                diff += DoublePi;

            if (diff > Pi)
                diff -= DoublePi;

            return diff;
        }

        public static float Square(this float value)
        {
            return value * value;
        }

        public static float Interpolate(float from, float to, float distance)
        {
            return from + distance * (to - from);
        }

        public static float ToRadians(this float value)
        {
            return (float)(value * ToRadiansConst);
        }

        public static double ToRadians(this double value)
        {
            return value * ToRadiansConst;
        }

        public static int RoundToInt(this double value)
        {
            return (int)Math.Round(value);
        }

        public static int RoundToInt(this float value)
        {
            return (int)Math.Round(value);
        }

        public static float Min(float x1, float x2, float x3, float x4)
        {
            return Math.Min(Math.Min(x1, x2), Math.Min(x3, x4));
        }

        public static float Max(float x1, float x2, float x3, float x4)
        {
            return Math.Max(Math.Max(x1, x2), Math.Max(x3, x4));
        }

        public static float Min(float x1, float x2, float x3)
        {
            return Math.Min(Math.Min(x1, x2), x3);
        }

        public static float Max(float x1, float x2, float x3)
        {
            return Math.Max(Math.Max(x1, x2), x3);
        }
    }
}

