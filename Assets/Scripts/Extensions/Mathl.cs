using System.Collections.Generic;
using UnityEngine;

namespace LPUnityUtils
{
    public static class Mathl
    {

        // Modulus operator that wraps at 0.
        public static int WrappingMod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static IEnumerable<float> DividedEvenly(float len, float interval, bool returnNormalized = false)
        {
            int sectionCount = Mathf.RoundToInt(len / interval);
            sectionCount = Mathf.Max(1, sectionCount);
            for ( int i = 0; i <= sectionCount; ++i )
            {
                float t = (float)(i) / sectionCount;
                if ( i == sectionCount )
                {
                    yield return returnNormalized ? 1f : len;
                }
                else
                {
                    yield return t * (returnNormalized ? 1f : len);
                }
            }
        }

        public static float RoundToPrecision(float value, float increment)
        {
            return Mathf.Round(value / increment) * increment;
        }

        // Return angle clamped between min and max.
        public static float ClampDegrees(float angle, float min, float max)
        {
            min = Mathf.Repeat(min, 360f);
            max = Mathf.Repeat(max, 360f);
            if (max < min)
            {
                max += 360.0f;
            }
            float rangeCenter = (min + max) * 0.5f;
            float maxDifference = (max - min) * 0.5f;
            return ClampDeltaDegrees(angle, rangeCenter, maxDifference);
        }

        // Return angle clamped so that it's at most maxDifference degrees away from rangeCenter.
        public static float ClampDeltaDegrees(float angle, float rangeCenter, float maxDifference)
        {
            float delta = Mathf.DeltaAngle(rangeCenter, angle);
            if ( delta < -maxDifference )
            {
                return rangeCenter - maxDifference;
            }
            if ( delta > maxDifference )
            {
                return rangeCenter + maxDifference;
            }
            return angle;
        }

        public static Vector2 RadiansToVector2(float radians)
        {
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        public static Vector2 DegreesToVector2(float degrees)
        {
            return RadiansToVector2(Mathf.Deg2Rad * degrees);
        }

        public static bool CircleCircleIntersection(Vector2 center1, float radius1, Vector2 center2, float radius2, out Vector2 intersection1, out Vector2 intersection2)
        {
            // https://stackoverflow.com/questions/3349125/circle-circle-intersection-points
            float distance = Vector2.Distance(center1, center2);

            if ( distance >= radius1 + radius2 || distance <= Mathf.Abs(radius1 - radius2))
            {
                intersection1 = Vector2.zero;
                intersection2 = Vector2.zero;
                return false;
            }
            if (distance < Mathf.Epsilon)
            {
                intersection1 = Vector2.zero;
                intersection2 = Vector2.zero;
                return false;
            }
            float a = (radius1 * radius1 - radius2 * radius2 + distance * distance) / (2 * distance);

            // Clamping to zero with Mathf.Max is used here to make this work even when numerical inaccuracy would make the result invalid.
            float h = Mathf.Sqrt(Mathf.Max(radius1 * radius1 - a * a, 0.0f));

            Vector2 p2 = center1 + a * (center2 - center1) / distance;
            Vector2 hStep = h * ((center2 - center1) / distance).RotatedClockwise();

            intersection1 = p2 + hStep;
            intersection2 = p2 - hStep;
            return true;
        }

        public static float LimitMagnitude(float value, float limit)
        {
            return Mathf.Sign(value) * Mathf.Min(Mathf.Abs(value), limit);
        }

        // Subtract s from the magnitued of value without crossing zero.
        public static float SubtractMagnitude(float value, float s)
        {
            return Mathf.Sign(value) * Mathf.Max(Mathf.Abs(value) - s, 0.0f);
        }

        public static float AverageAngle(IEnumerable<float> anglesDegrees)
        {
            float sinSum = 0.0f;
            float cosSum = 0.0f;
            foreach ( float degrees in anglesDegrees )
            {
                float radians = Mathf.Deg2Rad * degrees;
                sinSum += Mathf.Sin(radians);
                cosSum += Mathf.Cos(radians);
            }
            return Mathf.Rad2Deg * Mathf.Atan2(sinSum, cosSum);
        }
    }

}
