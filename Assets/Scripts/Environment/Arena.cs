using System;
using UnityEngine;
using Utility;

namespace Environment {
    public class Arena : MonoBehaviour {
        public static Arena Instance { get; private set; }

        [SerializeField]
        private Vector2[] _wallAnchors = new Vector2[4];
        
        [SerializeField, Min(0.1f)]
        private float _gridSize = 1f;
        
        private Bounds _bounds;
        private Cell[] _grid;

#if UNITY_EDITOR
        private void Reset() {
            _wallAnchors = new Vector2[4];
            _bounds = new Bounds();
            _wallAnchors = new[] { new Vector2(-1f, 1f), new Vector2(1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f) };

            for (var i = 0; i < 4; i++)
                _bounds.Encapsulate(_wallAnchors[i]);
        }
#endif

        private void Awake() {
            if (Instance != null) {
                Debug.LogError("There can only be one instance of Environment");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start() {
            _bounds = new Bounds();
            
            foreach (Vector2 wallAnchor in _wallAnchors)
                _bounds.Encapsulate(wallAnchor);

            CreateGrid();
        }

        private void CreateGrid() {
            Vector3 size = _bounds.size;
            Vector3 center = _bounds.center;
            int xCount = Mathf.CeilToInt(size.x / _gridSize);
            int yCount = Mathf.CeilToInt(size.y / _gridSize);
            _grid = new Cell[xCount * yCount];

            for (var y = 0; y < yCount; y++) {
                for (var x = 0; x < xCount; x++) {
                    Vector2 cellPosition = GetCellPositionAtIndex(x, y, center, xCount, yCount);
                    _grid[y * xCount + x] = new Cell { position = cellPosition };
                }
            }
            
            for (var y = 0; y < yCount; y++)
                for (var x = 0; x < xCount; x++)
                    _grid[y * xCount + x].traversableDirections = GetMoveDirectionAtIndex(x, y, xCount, yCount);
        }

        private Vector2 GetCellPositionAtIndex(int x, int y, Vector2 center, int xCount, int yCount) {
            float halfSizeX = xCount * _gridSize * 0.5f;
            float halfSizeY = yCount * _gridSize * 0.5f;
            float xPos = center.x - halfSizeX + x * _gridSize + _gridSize * 0.5f;
            float yPos = center.y - halfSizeY + y * _gridSize + _gridSize * 0.5f;
            return new Vector2(xPos, yPos);
        }
        
        private MoveDirection GetMoveDirectionAtIndex(int x, int y, int xCount, int yCount) {
            var traversableDirections = MoveDirection.None;
            
            foreach (MoveDirection value in Enum.GetValues(typeof(MoveDirection))) {
                if (value == MoveDirection.None) continue;
                
                (int xIndex, int yIndex) = GetIndexInMoveDirection(value, x, y);
                
                if (xIndex < 0 || xIndex >= xCount || yIndex < 0 || yIndex >= yCount) continue;
                
                if (Geometry.PointInPolygon(_grid[y * xCount + x].position, _wallAnchors))
                    traversableDirections |= value;
            }
            
            return traversableDirections;
        }
        
        private static (int, int) GetIndexInMoveDirection(MoveDirection moveDirection, int x, int y) {
            int xIndex = x;
            int yIndex = y;
            
            if (moveDirection.IsNortherly()) yIndex++;
            if (moveDirection.IsEasterly()) xIndex++;
            if (moveDirection.IsSoutherly()) yIndex--;
            if (moveDirection.IsWesterly()) xIndex--;
            
            return (xIndex, yIndex);
        }
    }
}
