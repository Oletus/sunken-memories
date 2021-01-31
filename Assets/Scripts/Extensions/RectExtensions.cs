using UnityEngine;

namespace LPUnityUtils
{
    public static class RectExtensions
    {
        public static bool IntersectRay(this Rect value, Ray2D ray, out float distance)
        {
            Vector2 invDir = new Vector2(1.0f / ray.direction.x, 1.0f / ray.direction.y);
            float tMinX, tMaxX, tMinY, tMaxY;
            if ( invDir.x > 0 )
            {
                tMinX = (value.xMin - ray.origin.x) * invDir.x;
                tMaxX = (value.xMax - ray.origin.x) * invDir.x;
            }
            else
            {
                tMinX = (value.xMax - ray.origin.x) * invDir.x;
                tMaxX = (value.xMin - ray.origin.x) * invDir.x;
            }
            if ( invDir.y > 0 )
            {
                tMinY = (value.yMin - ray.origin.y) * invDir.y;
                tMaxY = (value.yMax - ray.origin.y) * invDir.y;
            }
            else
            {
                tMinY = (value.yMax - ray.origin.y) * invDir.y;
                tMaxY = (value.yMin - ray.origin.y) * invDir.y;
            }
            if ( value.Contains(ray.origin) )
            {
                distance = Mathf.Min(tMaxX, tMaxY) * ray.direction.magnitude;
                return true;
            }
            if ( tMinY > tMaxX || tMinX > tMaxY )
            {
                distance = -1f;
                return false;
            }
            if ( tMinX < 0 )
            {
                distance = tMinY * ray.direction.magnitude;
            }
            else if ( tMinY < 0 )
            {
                distance = tMinX * ray.direction.magnitude;
            }
            else {
                distance = Mathf.Min(tMinX, tMinY) * ray.direction.magnitude;
            }
            return true;
        }
    }
}
