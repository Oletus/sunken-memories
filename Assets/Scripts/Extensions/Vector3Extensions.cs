using UnityEngine;

namespace LPUnityUtils
{

    public static class Vector3Extensions {

        public static Vector2 ToTopDownSpace(this Vector3 value)
        {
            return new Vector2(value.x, value.z);
        }

        public static Vector3 LimitMagnitude(this Vector3 value, float limit)
        {
            if ( value == Vector3.zero )
            {
                return value;
            }
            float mag = value.magnitude;
            mag = Mathf.Min(mag, limit);
            return value.normalized * mag;
        }

        public static bool IsInFrontOf(this Vector3 value, Transform transform, float maxAngleDegrees)
        {
            float angle = Vector3.Angle((value - transform.position), transform.forward);
            return angle <= maxAngleDegrees;
        }

    }

}
