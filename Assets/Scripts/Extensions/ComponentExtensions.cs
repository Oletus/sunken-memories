using UnityEngine;

namespace LPUnityUtils
{

    public static class ComponentExtensions {

        public static T GetComponentInParent<T>(this Component value, bool includeInactive) where T : Component
        {
            if (!includeInactive)
            {
                return value.GetComponentInParent<T>();
            }
            while ( value != null )
            {
                T comp = value.GetComponent<T>();
                if ( comp != null )
                {
                    return comp;
                }
                value = value.transform.parent;
            }
            return null;
        }

        public static bool IsInScene(this Component value, string sceneName)
        {
            return value.gameObject.scene.name == sceneName;
        }

    }

}
