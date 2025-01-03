using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Zombies.Environment.Editor {
    [CustomEditor(typeof(Arena), true), CanEditMultipleObjects]
    public partial class ArenaEditor : UnityEditor.Editor {
        private FieldInfo _boundsField;
        
        private void OnEnable() {
            _boundsField = typeof(Arena).GetField("_bounds", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void OnSceneGUI() {
            var arena = (Arena)target;
            DrawWalls(arena.Walls);
            UpdateWallAnchorsPositions(arena.Walls, arena);
            UpdateBounds(arena);
        }

        private static void DrawWalls(Polygon levelGeometry) {
            Handles.color = Color.blue;
            Vector2[] polygon = levelGeometry;
            
            for (var i = 0; i < polygon.Length - 1; i++) {
                Vector2 current = polygon[i];
                Vector2 next = polygon[i + 1];
                Handles.DrawLine(current, next);
            }
            
            Handles.DrawLine(polygon[^1], polygon[0]);
        }

        private void UpdateWallAnchorsPositions(Polygon levelGeometry, Arena arena) {
            Handles.color = Color.white;
            Vector2[] polygon = levelGeometry;
            
            for (var i = 0; i < polygon.Length; i++) {
                EditorGUI.BeginChangeCheck();
                float size = HandleUtility.GetHandleSize(polygon[i]) * 0.1f;
                Vector3 snap = EditorSnapSettings.snapEnabled ? EditorSnapSettings.move : Vector3.zero;
                Vector3 newPos = Handles.FreeMoveHandle(polygon[i], size, snap, Handles.SphereHandleCap);

                if (!EditorGUI.EndChangeCheck()) continue;
                
                Undo.RecordObject(arena, "Update Arena Wall Anchor");
                polygon[i] = newPos;
                EditorUtility.SetDirty(arena);
            }
        }

        private void UpdateBounds(Arena arena) {
            if (_boundsField == null) return;
            var bounds = new Bounds();
            
            foreach (Vector2 anchor in arena.Walls.vertices)
                bounds.Encapsulate(anchor);
            
            _boundsField.SetValue(arena, bounds);
            Handles.color = Color.white;
            Handles.DrawWireCube(bounds.center, bounds.size);
        }
    }
}