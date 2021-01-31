using UnityEngine;

namespace LPUnityUtils
{

    public static class Vector2IntExtensions {

        public static Vector3Int FromTopDownToWorldSpace(this Vector2Int value)
        {
            return new Vector3Int(value.x, 0, value.y);
        }

        // Rotates a direction vector 90 degrees counter-clockwise in a coordinate system where y points up and x points to the right.
        public static Vector2Int RotatedCounterClockwise(ref this Vector2Int value)
        {
            return new Vector2Int(-value.y, value.x);
        }

        // Rotates a direction vector 90 degrees clockwise in a coordinate system where y points up and x points to the right.
        public static Vector2Int RotatedClockwise(ref this Vector2Int value)
        {
            return new Vector2Int(value.y, -value.x);
        }

        public static string DirectionName(this Vector2Int value)
        {
            if ( value == Vector2Int.right )
            {
                return "right";
            }
            if ( value == Vector2Int.left )
            {
                return "left";
            }
            if ( value == Vector2Int.up )
            {
                return "up";
            }
            if ( value == Vector2Int.down )
            {
                return "down";
            }
            return "none";
        }

        // Returns results consistent with Vector2.SignedAngle(Vector2.right, direction);
        public static float DirectionToSignedAngle(this Vector2Int direction)
        {
            if ( direction == Vector2Int.left )
            {
                return 180f;
            }
            if ( direction == Vector2Int.up )
            {
                return 90f;
            }
            if ( direction == Vector2Int.down )
            {
                return -90f;
            }
            return 0f;
        }
    }

}
