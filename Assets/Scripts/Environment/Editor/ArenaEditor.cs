﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Environment.Editor {
    [CustomEditor(typeof(Arena), true), CanEditMultipleObjects]
    public class ArenaEditor : UnityEditor.Editor {
        private FieldInfo _levelGeometryField;
        private FieldInfo _gridSizeField;
        private FieldInfo _boundsField;
        private FieldInfo _gridField;
        
        private bool _drawCells;
        private bool _drawPositionHandles;
        
        private void OnEnable() {
            _levelGeometryField = typeof(Arena).GetField("_levelGeometry", BindingFlags.NonPublic | BindingFlags.Instance);
            _gridSizeField = typeof(Arena).GetField("_gridSize", BindingFlags.NonPublic | BindingFlags.Instance);
            _boundsField = typeof(Arena).GetField("_bounds", BindingFlags.NonPublic | BindingFlags.Instance);
            _gridField = typeof(Arena).GetField("_grid", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            _drawCells = EditorGUILayout.Toggle("Draw Cells", _drawCells);
            _drawPositionHandles = EditorGUILayout.Toggle("Draw Position Handles", _drawPositionHandles);
            
            var arena = (Arena)target;
            
            if (GUILayout.Button("Recreate Arena"))
                arena.RecreateArena();
        }

        private void OnSceneGUI() {
            var arena = (Arena)target;
            
            if (_levelGeometryField == null) return;
            
            var levelGeometry = (Polygon[])_levelGeometryField.GetValue(arena);
            if (levelGeometry == null) return;

            DrawWalls(levelGeometry);
            if (_drawPositionHandles) UpdateWallAnchorsPositions(levelGeometry, arena);
            UpdateBounds(arena, levelGeometry);
            if (_drawCells) DrawCells(arena);
        }

        private static void DrawWalls(Polygon[] levelGeometry) {
            foreach (Vector2[] polygon in levelGeometry) {
                Handles.color = Geometry.IsPolygonClockwise(polygon) ? Color.green : Color.red;
                
                for (var i = 0; i < polygon.Length; i++) {
                    Vector2 current = polygon[i];
                    Vector2 next = polygon[(i + 1) % polygon.Length];
                    Handles.DrawLine(current, next);
                    float diskSize = HandleUtility.GetHandleSize(current) * 0.075f;
                    Handles.DrawSolidDisc(current, Vector3.back, diskSize);
                    Handles.Label(current + Vector2.up * 0.4f, $"{i}");
                }
            }
        }

        private void UpdateWallAnchorsPositions(Polygon[] levelGeometry, Arena arena) {
            foreach (Vector2[] polygon in levelGeometry) {
                for (var i = 0; i < polygon.Length; i++) {
                    EditorGUI.BeginChangeCheck();
                    Vector3 newPos = Handles.PositionHandle(polygon[i], Quaternion.identity);

                    if (!EditorGUI.EndChangeCheck()) continue;
                    
                    // if (_gridSizeField != null) {
                    //     var gridSize = (float)_gridSizeField.GetValue(arena);
                    //     newPos.x = Mathf.Round(newPos.x / gridSize) * gridSize;
                    //     newPos.y = Mathf.Round(newPos.y / gridSize) * gridSize;
                    // }

                    Undo.RecordObject(arena, "Update Arena Wall Anchor");
                    polygon[i] = newPos;
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
                
                // foreach (MoveDirection value in Enum.GetValues(typeof(MoveDirection))) {
                //     if (value == MoveDirection.None) continue;
                //     if (!cell.traversableDirections.HasFlag(value)) continue;
                //
                //     // Hacky but it works
                //     Vector2 direction = value switch {
                //         MoveDirection.North => Vector2.up,
                //         MoveDirection.NorthEast => Vector2.one,
                //         MoveDirection.East => Vector2.right,
                //         MoveDirection.SouthEast => new Vector2(1, -1),
                //         MoveDirection.South => Vector2.down,
                //         MoveDirection.SouthWest => new Vector2(-1, -1),
                //         MoveDirection.West => Vector2.left,
                //         MoveDirection.NorthWest => new Vector2(-1, 1),
                //         _ => Vector2.zero
                //     };
                //         
                //     Handles.DrawLine(cell.position, cell.position + direction * gridSize);
                // }
            }
        }
    }
}