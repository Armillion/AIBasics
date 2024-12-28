using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utility;
using Utility.GizmosLegend;
using GizmoType = Utility.GizmosLegend.GizmoType;

namespace Shooter.Environment.Editor {
    [CustomEditor(typeof(Arena), true), CanEditMultipleObjects]
    public class ArenaEditor : UnityEditor.Editor {
        private FieldInfo _levelGeometryField;
        private FieldInfo _wallsField;
        private PropertyInfo _gridSizeProperty;
        private FieldInfo _boundsField;

        private bool _drawCells;

        private void OnEnable() {
            _levelGeometryField = typeof(Arena).GetField(
                "_levelGeometry", BindingFlags.NonPublic | BindingFlags.Instance
            );

            _wallsField = typeof(Arena).GetField("_walls", BindingFlags.NonPublic | BindingFlags.Instance);
            _gridSizeProperty = typeof(Arena).GetProperty("GridSize", BindingFlags.NonPublic | BindingFlags.Instance);
            _boundsField = typeof(Arena).GetField("_bounds", BindingFlags.NonPublic | BindingFlags.Instance);
            
            GizmosLegend.AddLabel((Arena)target, "Clockwise Geometry", Color.green, GizmoType.LineStrip);
            GizmosLegend.AddLabel((Arena)target, "Counter-Clockwise Geometry", Color.red, GizmoType.LineStrip);
            GizmosLegend.AddLabel((Arena)target, "Walls", Color.blue, GizmoType.LineStrip);
        }

        private void OnDisable() {
            GizmosLegend.Unregister((Arena)target);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            _drawCells = EditorGUILayout.Toggle("Draw Cells", _drawCells);
            var arena = (Arena)target;

            if (GUILayout.Button("Recreate Arena"))
                arena.RecreateArena();
        }

        private void OnSceneGUI() {
            var arena = (Arena)target;

            if (_levelGeometryField != null) {
                var levelGeometry = (Polygon[])_levelGeometryField.GetValue(arena);
                if (levelGeometry == null) return;
                DrawWalls(levelGeometry, true);

                UpdateWallAnchorsPositions(levelGeometry, arena);
                UpdateBounds(arena, levelGeometry);
            }

            if (_wallsField != null) {
                var walls = (Polygon[])_wallsField.GetValue(arena);
                if (walls == null) return;
                DrawWalls(walls, false);
                UpdateWallAnchorsPositions(walls, arena);
            }

            if (_drawCells) {
                GizmosLegend.AddLabel((Arena)target, "Traversable Cell", Color.gray, GizmoType.WireDisc);
                GizmosLegend.AddLabel((Arena)target, "Traversable Direction", Color.white, GizmoType.Line);
                DrawCells(arena);
            } else {
                GizmosLegend.RemoveLabel((Arena)target, "Traversable Cell");
                GizmosLegend.RemoveLabel((Arena)target, "Traversable Direction");
            }
        }

        private static void DrawWalls(Polygon[] levelGeometry, bool isClosedShape) {
            Handles.color = Color.blue;

            foreach (Vector2[] polygon in levelGeometry) {
                if (isClosedShape)
                    Handles.color = Geometry.IsPolygonClockwise(polygon) ? Color.green : Color.red;

                for (var i = 0; i < polygon.Length - 1; i++) {
                    Vector2 current = polygon[i];
                    Vector2 next = polygon[i + 1];
                    Handles.DrawLine(current, next);

                    if (isClosedShape)
                        Handles.Label(current + Vector2.up * 0.4f, $"{i}");
                }

                if (isClosedShape) Handles.DrawLine(polygon[^1], polygon[0]);
            }
        }

        private static void UpdateWallAnchorsPositions(Polygon[] levelGeometry, Arena arena) {
            Handles.color = Color.white;

            foreach (Vector2[] polygon in levelGeometry) {
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
        }

        private void UpdateBounds(Arena arena, Polygon[] levelGeometry) {
            if (_boundsField == null) return;

            var bounds = new Bounds();

            foreach (Vector2[] polygon in levelGeometry)
                foreach (Vector2 anchor in polygon)
                    bounds.Encapsulate(anchor);

            _boundsField.SetValue(arena, bounds);
            Handles.color = Color.white;
            Handles.DrawWireCube(bounds.center, bounds.size);
        }

        private void DrawCells(Arena arena) {
            if (arena.Grid == null) return;

            float gridSize = _gridSizeProperty != null ? (float)_gridSizeProperty.GetValue(arena) : 0.1f;

            foreach (Cell cell in arena.Grid) {
                if (cell.TraversableDirections == MoveDirection.None) continue;
                if (!IsPointInSceneViewCameraViewport(cell.position)) continue;
                
                Handles.color = Color.gray;
                Handles.DrawWireDisc(cell.position, Vector3.forward, gridSize * 0.5f);
                Handles.color = Color.white;

                foreach (MoveDirection value in Enum.GetValues(typeof(MoveDirection))) {
                    if (value == MoveDirection.None) continue;
                    if (!cell.TraversableDirections.HasFlag(value)) continue;

                    // Hacky but it works
                    Vector2 direction = value switch {
                        MoveDirection.North => Vector2.up,
                        MoveDirection.NorthEast => Vector2.one,
                        MoveDirection.East => Vector2.right,
                        MoveDirection.SouthEast => new Vector2(1, -1),
                        MoveDirection.South => Vector2.down,
                        MoveDirection.SouthWest => new Vector2(-1, -1),
                        MoveDirection.West => Vector2.left,
                        MoveDirection.NorthWest => new Vector2(-1, 1),
                        _ => Vector2.zero
                    };

                    Handles.DrawLine(cell.position, cell.position + direction * gridSize);
                }
            }
        }

        private static bool IsPointInSceneViewCameraViewport(Vector3 point) {
            if (!SceneView.lastActiveSceneView)
                return false;

            Camera sceneCamera = SceneView.lastActiveSceneView.camera;

            if (!sceneCamera)
                return false;

            Vector3 viewportPoint = sceneCamera.WorldToViewportPoint(point);
            return viewportPoint is { z: > 0, x: >= 0 and <= 1, y: >= 0 and <= 1 };
        }
    }
}