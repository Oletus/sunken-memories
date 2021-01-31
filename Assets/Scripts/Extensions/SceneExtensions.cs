using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LPUnityUtils
{

    public static class SceneExtensions {

        public static T GetComponentInRootGameObjects<T>(this Scene value) where T : Component
        {
            foreach ( GameObject obj in value.GetRootGameObjects() )
            {
                T comp = obj.GetComponent<T>();
                if (comp != null)
                {
                    return comp;
                }
            }
            return null;
        }

        public static IEnumerable<T> GetComponentsInRootGameObjects<T>(this Scene value) where T : Component
        {
            foreach ( GameObject obj in value.GetRootGameObjects() )
            {
                foreach ( T comp in obj.GetComponents<T>() )
                {
                    yield return comp;
                }
            }
        }

    }

}
