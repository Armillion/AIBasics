using System;
using System.Linq;
using UnityEngine;
using Utility;

namespace Environment {
    public class Arena : MonoBehaviour {
        public static Arena Instance { get; private set; }

        [SerializeField]
        private Polygon[] _levelGeometry = new Polygon[1];
        
        [SerializeField]
        private Polygon[] _walls = new Polygon[1];
        
        [SerializeField, Min(0.1f)]
        private float _gridSize = 1f;
        
        private Bounds _bounds;
        private Cell[] _grid;

#if UNITY_EDITOR
        private void Reset() {
            _bounds = new Bounds();
            _levelGeometry = new Polygon[1];
            _levelGeometry[0] = new[] { new Vector2(-1f, 1f), new Vector2(1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f) };
            _walls = Array.Empty<Polygon>();
            
            for (var i = 0; i < 4; i++)
                _bounds.Encapsulate(_levelGeometry[0][i]);
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
            RecreateArena();
        }

        public void RecreateArena() {
            _bounds = new Bounds();

            foreach (Vector2[] polygon in _levelGeometry)
                foreach (Vector2 wallAnchor in polygon)
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
            Vector2[][] levelGeometry = _levelGeometry.Select(poly => poly.vertices).ToArray();

            if (!Geometry.IsPointInPolygon(_grid[y * xCount + x].position, levelGeometry))
                return MoveDirection.None;
            
            var traversableDirections = MoveDirection.All;
            
            foreach (MoveDirection testedDirection in Enum.GetValues(typeof(MoveDirection))) {
                if (testedDirection is MoveDirection.None or MoveDirection.All) continue;
                (int xIndex, int yIndex) = GetIndexInMoveDirection(testedDirection, x, y);

                if (!IsValidIndex(xIndex, yIndex, xCount, yCount)) {
                    traversableDirections &= ~testedDirection;
                    continue;
                }
                
                Vector2 startPosition = _grid[y * xCount + x].position;
                Vector2 endPosition = _grid[yIndex * xCount + xIndex].position;

                if (!IsDirectionTraversable(testedDirection, startPosition, endPosition))
                    traversableDirections &= ~testedDirection;
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
        
        private static bool IsValidIndex(int xIndex, int yIndex, int xCount, int yCount)
            => xIndex >= 0 && xIndex < xCount && yIndex >= 0 && yIndex < yCount;

        private bool IsDirectionTraversable(MoveDirection direction, Vector2 start, Vector2 end) {
            foreach (Polygon wall in _walls)
                if (Geometry.LinesIntersect(start, end, wall, false))
                    return false;

            foreach (Polygon wall in _levelGeometry)
                if (Geometry.LinesIntersect(start, end, wall, true))
                    return false;

            return true;
        }
    }
}
