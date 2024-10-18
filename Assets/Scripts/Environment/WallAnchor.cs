using UnityEditor;
using UnityEngine;

namespace Environment {
    public class WallAnchor : MonoBehaviour {
#if UNITY_EDITOR
        private void Reset() {
            GUIContent iconContent = EditorGUIUtility.IconContent("sv_icon_dot9_sml");
            EditorGUIUtility.SetIconForObject(gameObject, (Texture2D) iconContent.image);
        }
#endif
    }
}
