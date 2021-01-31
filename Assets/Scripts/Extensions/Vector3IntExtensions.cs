using UnityEngine;

namespace LPUnityUtils
{

    public static class Vector3IntExtensions {

        public static Vector3Int Cross(this Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.y * b.z - a.z * b.y,
                                  a.z * b.x - a.x * b.z,
                                  a.x * b.y - a.y * b.x);
        }

        public static Vector2Int ToTopDownSpace(this Vector3Int value)
        {
            return new Vector2Int(value.x, value.z);
        }

        public static string DirectionName(this Vector3Int value)
        {
            if ( value == Vector3Int.right )
            {
                return "right";
            }
            if ( value == Vector3Int.left )
            {
                return "left";
            }
            if ( value == Vector3Int.up )
            {
                return "up";
            }
            if ( value == Vector3Int.down )
            {
                return "down";
            }
            if ( value == new Vector3Int(0, 0, -1) )
            {
                return "back";
            }
            if ( value == new Vector3Int(0, 0, 1) )
            {
                return "forward";
            }
            return "none";
        }

    }

}
