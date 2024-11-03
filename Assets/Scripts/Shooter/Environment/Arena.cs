using System;
using System.Linq;
using UnityEngine;
using Utility;

namespace Shooter.Environment {
    public class Arena : MonoBehaviour {
        [SerializeField]
        private Polygon[] _levelGeometry = new Polygon[1];
        
        [SerializeField]
        private Polygon[] _walls = new Polygon[1];
        
        [SerializeField, Min(0.1f)]
        private float _gridSize = 1f;
        
        [field: SerializeField]
        public Cell[] Grid { get; private set; }
        public int XCellCount { get; private set; }
        public int YCellCount { get; private set; }
        
        private Bounds _bounds;

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
            RecreateArena();
        }

        public void RecreateArena() {
            _bounds = new Bounds();

            foreach (Vector2[] polygon in _levelGeometry)
                foreach (Vector2 wallAnchor in polygon)
                    _bounds.Encapsulate(wallAnchor);

            CreateGrid();
        }

        public bool TryGetValidCellPosition(ref Vector2 position) {
            (int x, int y) = GetCellIndex(position);

            if (!IsValidIndex(x, y))
                return false;
            
            position = Grid[y * XCellCount + x].position;
            return true;
        }

        public bool TryGetCellIndex(Vector2 position, out int cellIndex) {
            (int x, int y) = GetCellIndex(position);
            cellIndex = y * XCellCount + x;
            return IsValidIndex(x, y);
        }
        
        private void CreateGrid() {
            Vector3 size = _bounds.size;
            Vector3 center = _bounds.center;
            XCellCount = Mathf.CeilToInt(size.x / _gridSize);
            YCellCount = Mathf.CeilToInt(size.y / _gridSize);
            Grid = new Cell[XCellCount * YCellCount];

            for (var y = 0; y < YCellCount; y++) {
                for (var x = 0; x < XCellCount; x++) {
                    Vector2 cellPosition = GetCellPositionAtIndex(x, y, center);
                    Grid[y * XCellCount + x] = new Cell { position = cellPosition };
                }
            }
            
            for (var y = 0; y < YCellCount; y++)
                for (var x = 0; x < XCellCount; x++)
                    Grid[y * XCellCount + x].TraversableDirections = GetMoveDirectionAtIndex(x, y);
        }

        private Vector2 GetCellPositionAtIndex(int x, int y, Vector2 center) {
            float halfSizeX = XCellCount * _gridSize * 0.5f;
            float halfSizeY = YCellCount * _gridSize * 0.5f;
            float xPos = center.x - halfSizeX + x * _gridSize + _gridSize * 0.5f;
            float yPos = center.y - halfSizeY + y * _gridSize + _gridSize * 0.5f;
            return new Vector2(xPos, yPos);
        }
        
        private MoveDirection GetMoveDirectionAtIndex(int x, int y) {
            Vector2[][] levelGeometry = _levelGeometry.Select(poly => poly.vertices).ToArray();

            if (!Geometry.IsPointInPolygon(Grid[y * XCellCount + x].position, levelGeometry))
                return MoveDirection.None;
            
            var traversableDirections = MoveDirection.All;
            
            foreach (MoveDirection testedDirection in Enum.GetValues(typeof(MoveDirection))) {
                if (testedDirection is MoveDirection.None or MoveDirection.All) continue;
                (int xIndex, int yIndex) = GetIndexInMoveDirection(testedDirection, x, y);

                if (!IsValidIndex(xIndex, yIndex)) {
                    traversableDirections &= ~testedDirection;
                    continue;
                }
                
                Vector2 startPosition = Grid[y * XCellCount + x].position;
                Vector2 endPosition = Grid[yIndex * XCellCount + xIndex].position;

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
        
        private bool IsValidIndex(int xIndex, int yIndex)
            => xIndex >= 0 && xIndex < XCellCount && yIndex >= 0 && yIndex < YCellCount;

        private bool IsDirectionTraversable(MoveDirection direction, Vector2 start, Vector2 end) {
            foreach (Polygon wall in _walls)
                if (Geometry.LinesIntersect(start, end, wall, false))
                    return false;

            foreach (Polygon wall in _levelGeometry)
                if (Geometry.LinesIntersect(start, end, wall, true))
                    return false;

            return true;
        }
                
        private (int, int) GetCellIndex(Vector2 position) {
            Vector2 localPosition = position - (Vector2)transform.position;
            Vector2 size = _bounds.size;
            Vector2 center = _bounds.center;
            int x = Mathf.FloorToInt((localPosition.x - center.x + size.x * 0.5f) / _gridSize);
            int y = Mathf.FloorToInt((localPosition.y - center.y + size.y * 0.5f) / _gridSize);
            return (x, y);
        }
    }
}
