using System;
using System.Linq;
using Physics;
using Shooter.Agents;
using UnityEngine;
using Utility;

namespace Shooter.Environment {
    public class Arena : MonoBehaviour {
        [SerializeField]
        private Polygon[] _levelGeometry = new Polygon[1];
        
        [SerializeField]
        private Polygon[] _walls = new Polygon[1];
        
        [SerializeField]
        private AgentConfig _agentConfig;
        
        [field: SerializeField]
        public Cell[] Grid { get; private set; }
        public int XCellCount { get; private set; }
        public int YCellCount { get; private set; }

        private float GridSize => _agentConfig ? _agentConfig.Radius * 2f : 1f;
        
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
            SimplePhysics2D.RegisterGeometry(_levelGeometry, _walls);
        }

        public void RecreateArena() {
            _bounds = new Bounds();

            foreach (Vector2[] polygon in _levelGeometry)
                foreach (Vector2 wallAnchor in polygon)
                    _bounds.Encapsulate(wallAnchor);

            CreateGrid();
        }

        public int SnapPositionToCell(ref Vector2 position) {
            (int x, int y) = GetCellIndex(position);

            if (!IsValidIndex(x, y))
                return -1;

            int index = y * XCellCount + x;
            
            if (Grid[index].TraversableDirections == MoveDirection.None)
                return -1;
            
            position = Grid[index].position;
            return index;
        }

        public bool TryGetCellIndex(Vector2 position, out int cellIndex) {
            (int x, int y) = GetCellIndex(position);
            cellIndex = y * XCellCount + x;
            return IsValidIndex(x, y);
        }
        
        public Vector2 GetNeighbouringCellPosition(Vector2 position) {
            (int x, int y) = GetCellIndex(position);
            Cell cell = Grid[y * XCellCount + x];
            
            if (cell.TraversableDirections == MoveDirection.None)
                return position;
            
            MoveDirection randomDirection;
            
            do {
                int randomBitShift = UnityEngine.Random.Range(0, 7);
                randomDirection = (MoveDirection)(1 << randomBitShift);
            } while ((randomDirection & cell.TraversableDirections) == MoveDirection.None);
            
            (int xIndex, int yIndex) = GetIndexInMoveDirection(randomDirection, x, y);
            return IsValidIndex(xIndex, yIndex) ? Grid[yIndex * XCellCount + xIndex].position : position;
        }

        private bool IsStraightPathTraversable(Vector2 start, Vector2 end) {
            foreach (Polygon wall in _walls)
                if (Geometry.LinesIntersect(start, end, wall, false, out _))
                    return false;

            foreach (Polygon wall in _levelGeometry)
                if (Geometry.LinesIntersect(start, end, wall, true, out _))
                    return false;

            return true;
        }

        private void CreateGrid() {
            Vector3 size = _bounds.size;
            Vector3 center = _bounds.center;
            XCellCount = Mathf.CeilToInt(size.x / GridSize);
            YCellCount = Mathf.CeilToInt(size.y / GridSize);
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
            
            for (var y = 0; y < YCellCount; y++)
                for (var x = 0; x < XCellCount; x++)
                    CheckCellForAgentCollision(x, y);
        }

        private Vector2 GetCellPositionAtIndex(int x, int y, Vector2 center) {
            float halfSizeX = XCellCount * GridSize * 0.5f;
            float halfSizeY = YCellCount * GridSize * 0.5f;
            float xPos = center.x - halfSizeX + x * GridSize + GridSize * 0.5f;
            float yPos = center.y - halfSizeY + y * GridSize + GridSize * 0.5f;
            return new Vector2(xPos, yPos);
        }
        
        private MoveDirection GetMoveDirectionAtIndex(int x, int y) {
            Vector2[][] levelGeometry = _levelGeometry.Select(poly => poly.vertices).ToArray();
            Vector2 cellPosition = Grid[y * XCellCount + x].position;
            
            if (!Geometry.IsPointInPolygon(cellPosition, levelGeometry))
                return MoveDirection.None;
            
            var traversableDirections = MoveDirection.All;
            
            foreach (MoveDirection testedDirection in Enum.GetValues(typeof(MoveDirection))) {
                if (testedDirection is MoveDirection.None or MoveDirection.All) continue;
                (int xIndex, int yIndex) = GetIndexInMoveDirection(testedDirection, x, y);

                if (!IsValidIndex(xIndex, yIndex)) {
                    traversableDirections &= ~testedDirection;
                    continue;
                }
                
                Vector2 endPosition = Grid[yIndex * XCellCount + xIndex].position;

                if (!IsStraightPathTraversable(cellPosition, endPosition))
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

        private (int, int) GetCellIndex(Vector2 position) {
            Vector2 localPosition = position - (Vector2)transform.position;
            Vector2 size = _bounds.size;
            Vector2 center = _bounds.center;
            int x = Mathf.FloorToInt((localPosition.x - center.x + size.x * 0.5f) / GridSize);
            int y = Mathf.FloorToInt((localPosition.y - center.y + size.y * 0.5f) / GridSize);
            return (x, y);
        }

        private void CheckCellForAgentCollision(int x, int y) {
            Vector2[][] levelGeometry = _levelGeometry.Select(poly => poly.vertices).ToArray();
            int cellIndex = y * XCellCount + x;
            Vector2 cellPosition = Grid[cellIndex].position;
            
            if (Geometry.CircleIntersectsPolygon(cellPosition, GridSize * 0.5f, levelGeometry))
                MarkCellNotTraversable(x, y);
            
            Vector2[][] walls = _walls.Select(poly => poly.vertices).ToArray();
            
            if (Geometry.CircleIntersectsPolygon(cellPosition, GridSize * 0.5f, walls, false))
                MarkCellNotTraversable(x, y);
        }

        private void MarkCellNotTraversable(int x, int y) {
            int index = y * XCellCount + x;
            Grid[index].TraversableDirections = MoveDirection.None;
            
            foreach (MoveDirection testedDirection in Enum.GetValues(typeof(MoveDirection))) {
                if (testedDirection is MoveDirection.None or MoveDirection.All) continue;
                (int xIndex, int yIndex) = GetIndexInMoveDirection(testedDirection, x, y);

                if (!IsValidIndex(xIndex, yIndex)) continue;
                
                int adjacentIndex = yIndex * XCellCount + xIndex;
                Grid[adjacentIndex].TraversableDirections &= ~testedDirection.Opposite();
            }
        }
    }
}
