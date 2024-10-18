using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Environment.Editor {
    [CustomEditor(typeof(Arena), true), CanEditMultipleObjects]
    public class ArenaEditor : UnityEditor.Editor {
        private FieldInfo _wallAnchorsField;
        private FieldInfo _gridSizeField;
        private FieldInfo _boundsField;
        private FieldInfo _gridField;
        
        private void OnEnable() {
            _wallAnchorsField = typeof(Arena).GetField("_wallAnchors", BindingFlags.NonPublic | BindingFlags.Instance);
            _gridSizeField = typeof(Arena).GetField("_gridSize", BindingFlags.NonPublic | BindingFlags.Instance);
            _boundsField = typeof(Arena).GetField("_bounds", BindingFlags.NonPublic | BindingFlags.Instance);
            _gridField = typeof(Arena).GetField("_grid", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void OnSceneGUI() {
            var arena = (Arena)target;
            
            if (_wallAnchorsField == null) return;
            
            var wallAnchors = (Vector2[])_wallAnchorsField.GetValue(arena);
            if (wallAnchors == null || wallAnchors.Length < 2) return;

            DrawWalls(wallAnchors);
            UpdateWallAnchorsPositions(wallAnchors, arena);
            UpdateBounds(arena, wallAnchors);
            DrawCells(arena);
        }

        private static void DrawWalls(Vector2[] wallAnchors) {
            Handles.color = Color.cyan;

            for (var i = 0; i < wallAnchors.Length; i++) {
                Vector2 current = wallAnchors[i];
                Vector2 next = wallAnchors[(i + 1) % wallAnchors.Length];
                Handles.DrawLine(current, next);
                Handles.Label(current + Vector2.up * 0.4f, $"{i}");
            }
        }

        private void UpdateWallAnchorsPositions(Vector2[] wallAnchors, Arena arena) {
            for (var i = 0; i < wallAnchors.Length; i++) {
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(wallAnchors[i], Quaternion.identity);

                if (!EditorGUI.EndChangeCheck()) continue;
                
                if (_gridSizeField != null) {
                    var gridSize = (float)_gridSizeField.GetValue(arena);
                    newPos.x = Mathf.Round(newPos.x / gridSize) * gridSize;
                    newPos.y = Mathf.Round(newPos.y / gridSize) * gridSize;
                }

                Undo.RecordObject(arena, "Update Arena Wall Anchor");
                wallAnchors[i] = newPos;
            }
        }

        private void UpdateBounds(Arena arena, Vector2[] wallAnchors) {
            if (_boundsField == null) return;

            var bounds = new Bounds();
            
            foreach (Vector2 anchor in wallAnchors)
                bounds.Encapsulate(anchor);
            
            _boundsField.SetValue(arena, bounds);
            Handles.color = Color.blue;
            Handles.DrawWireCube(bounds.center, bounds.size);
        }
        
        private void DrawCells(Arena arena) {
            if (_gridField == null) return;
            
            var grid = (Cell[])_gridField.GetValue(arena);
            if (grid == null) return;
            
            float gridSize = _gridSizeField != null ? (float)_gridSizeField.GetValue(arena) : 0.1f;

            foreach (Cell cell in grid) {
                Handles.color = cell.traversableDirections == MoveDirection.None ? Color.red : Color.green;
                Handles.DrawWireCube(cell.position, Vector3.one * 0.1f);
                
                foreach (MoveDirection value in Enum.GetValues(typeof(MoveDirection))) {
                    if (value == MoveDirection.None) continue;
                    if (!cell.traversableDirections.HasFlag(value)) continue;

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
    }
}