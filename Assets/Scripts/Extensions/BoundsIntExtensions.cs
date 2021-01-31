
using UnityEngine;

namespace LPUnityUtils
{
    public static class BoundsIntExtensions
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        public static void Encapsulate(ref this BoundsInt boundsThis, Vector3Int coords)
        {
            Vector3Int newMin = Vector3Int.Min(boundsThis.min, coords);
            Vector3Int newMax = Vector3Int.Max(boundsThis.max, coords);
            boundsThis.SetMinMax(newMin, newMax);
        }

        public static void Encapsulate(ref this BoundsInt boundsThis, BoundsInt bounds)
        {
            Vector3Int newMin = Vector3Int.Min(boundsThis.min, bounds.min);
            Vector3Int newMax = Vector3Int.Max(boundsThis.max, bounds.max);
            boundsThis.SetMinMax(newMin, newMax);
        }

        public static bool ContainsInclusive(this BoundsInt boundsThis, Vector3Int coords)
        {
            return (boundsThis.xMax >= coords.x && boundsThis.yMax >= coords.y && boundsThis.zMax >= coords.z) &&
                (boundsThis.xMin <= coords.x && boundsThis.yMin <= coords.y && boundsThis.zMin <= coords.z);
        }

        public static bool IntersectsInclusive(this BoundsInt boundsThis, BoundsInt bounds)
        {
            return
                (boundsThis.xMin <= bounds.xMax) && (boundsThis.xMax >= bounds.xMin) &&
                (boundsThis.yMin <= bounds.yMax) && (boundsThis.yMax >= bounds.yMin) &&
                (boundsThis.zMin <= bounds.zMax) && (boundsThis.zMax >= bounds.zMin);
        }

        public static BoundsInt GetIntersection(this BoundsInt boundsThis, BoundsInt bounds)
        {
            if ( !IntersectsInclusive(boundsThis, bounds) )
            {
                return new BoundsInt();
            }
            Vector3Int minCorner = Vector3Int.Max(boundsThis.min, bounds.min);
            return new BoundsInt(minCorner, Vector3Int.Min(boundsThis.max, bounds.max) - minCorner);
        }

        public static int GetSurfaceArea(this BoundsInt boundsThis)
        {
            return 2 * (boundsThis.size.x * boundsThis.size.y + boundsThis.size.x * boundsThis.size.z + boundsThis.size.y * boundsThis.size.z);
        }

        public static Axis GetBiggestAxis(this BoundsInt boundsThis)
        {
            float axis_x = boundsThis.size.x;
            float axis_y = boundsThis.size.y;
            float axis_z = boundsThis.size.z;

            // Return the biggest axis.
            if ( axis_x > axis_y )
            {
                if ( axis_x > axis_z )
                {
                    return Axis.X;
                }
                else
                {
                    return Axis.Z;
                }
            }
            else
            {
                if ( axis_y > axis_z )
                {
                    return Axis.Y;
                }
                else
                {
                    return Axis.Z;
                }
            }
        }

        public static Axis NextAxis(Axis cur)
        {
            switch ( cur )
            {
                case Axis.X: return Axis.Y;
                case Axis.Y: return Axis.Z;
                default: return Axis.X;
            }
        }

    }

}
