using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LPUnityUtils
{

    public static class TransformExtensions {

        public static void Swap(this Transform valueA, Transform valueB)
        {
            Vector3 oldAPosition = valueA.position;
            Quaternion oldARotation = valueA.rotation;
            Vector3 oldAScale = valueA.localScale;
            Transform oldAParent = valueA.parent;
            valueA.parent = valueB.parent;
            valueA.SetPositionAndRotation(valueB.position, valueB.rotation);
            valueA.localScale = valueB.localScale;

            valueB.parent = oldAParent;
            valueB.SetPositionAndRotation(oldAPosition, oldARotation);
            valueB.localScale = oldAScale;
        }

        public static GameObject CreateEmptyChild(this Transform parent, string name="New GameObject")
        {
            GameObject go = new GameObject();
            go.name = name;
            if ( go.scene != parent.gameObject.scene )
            {
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, parent.gameObject.scene);
            }
            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go;
        }

        public static IEnumerable<GameObject> AllChildrenGameObjects(this Transform parent)
        {
            foreach ( Transform child in parent )
            {
                yield return child.gameObject;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/Transform/Align selected to grid on X axis", false, 150)]
        private static void AlignSelectedX()
        {
            AlignSelectedOnAxis(0);
        }

        [UnityEditor.MenuItem("CONTEXT/Transform/Align selected to grid on Y axis", false, 150)]
        private static void AlignSelectedY()
        {
            AlignSelectedOnAxis(1);
        }

        private static void AlignSelectedOnAxis(int axisIndex)
        {
            List<Transform> transforms = UnityEditor.Selection.transforms.ToList();
            if ( transforms.Count <= 1 )
            {
                return;
            }
            transforms = transforms.OrderBy((Transform x) => x.position[axisIndex]).ToList();

            float startX = transforms[0].position[axisIndex];
            float endX = transforms[transforms.Count - 1].position[axisIndex];
            float intervalX = (endX - startX) / (transforms.Count - 1);

            float currentX = startX;
            foreach ( Transform tr in transforms )
            {
                UnityEditor.Undo.RecordObject(tr, "Align selected");
                Vector3 pos = tr.position;
                pos[axisIndex] = currentX;
                tr.position = pos;
                currentX += intervalX;
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(tr);
            }
        }
#endif
    }

}
