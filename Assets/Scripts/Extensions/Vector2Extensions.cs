using UnityEngine;

namespace LPUnityUtils
{

    public static class Vector2Extensions
    {

        public static Vector3 FromTopDownToWorldSpace(this Vector2 value)
        {
            return new Vector3(value.x, 0, value.y);
        }

        public static void Clamp(ref this Vector2 vector, Vector2 min, Vector2 max)
        {
            vector.x = Mathf.Clamp(vector.x, min.x, max.x);
            vector.y = Mathf.Clamp(vector.y, min.y, max.y);
        }

        public static Vector2 Clamped(this Vector2 vector, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y));
        }

        public static Vector2 LimitMagnitude(this Vector2 value, float limit)
        {
            if ( value == Vector2.zero )
            {
                return value;
            }
            float mag = value.magnitude;
            mag = Mathf.Min(mag, limit);
            return value.normalized * mag;
        }

        public static Vector2 ClosestPointInsideCircle(this Vector2 value, Vector2 circleCenter, float circleRadius)
        {
            if ( Vector2.Distance(value, circleCenter) > circleRadius )
            {
                return circleCenter + (value - circleCenter).normalized * circleRadius;
            }
            return value;
        }

        public static Vector2 NearestCardinalDirection(this Vector2 value)
        {
            if ( Mathf.Abs(value.x) > Mathf.Abs(value.y) )
            {
                return value.x >= 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                return value.y >= 0 ? Vector2.up : Vector2.down;
            }
        }

        // Rotates a direction vector 90 degrees counter-clockwise in a coordinate system where y points up and x points to the right.
        public static Vector2 RotatedCounterClockwise(this Vector2 value)
        {
            return new Vector2(-value.y, value.x);
        }

        // Rotates a direction vector 90 degrees clockwise in a coordinate system where y points up and x points to the right.
        public static Vector2 RotatedClockwise(this Vector2 value)
        {
            return new Vector2(value.y, -value.x);
        }

    }

}
