using System.Collections.Generic;
using UnityEngine;

namespace SpacePartitioning {
    public class CellSpacePartition<T> where T : MonoBehaviour {
        private readonly Vector2 _center;
        private readonly Vector2 _spaceSize;
        private readonly Vector2Int _numCells;
        private readonly List<T>[] _cells;
        private readonly HashSet<T> _entities = new();

        public CellSpacePartition(Vector2 center, Vector2 spaceSize, Vector2Int numCells) {
            _center = center;
            _spaceSize = spaceSize;
            _numCells = numCells;
            _cells = new List<T>[numCells.x * numCells.y];
        }
        
        public CellSpacePartition(Vector2 center, Vector2 spaceSize, float cellSize) {
            _center = center;
            _spaceSize = spaceSize;
            
            _numCells = new Vector2Int(
                Mathf.CeilToInt(spaceSize.x / cellSize),
                Mathf.CeilToInt(spaceSize.y / cellSize)
            );
         
            _cells = new List<T>[_numCells.x * _numCells.y];
        }
        
        public void UpdatePositions() {
            foreach (List<T> cell in _cells)
                cell?.Clear();

            foreach (List<T> cell in _cells)
                cell?.Clear();

            foreach (T entity in _entities) {
                int cellIndex = PositionToIndex(entity.transform.position);
                _cells[cellIndex] ??= new List<T>();
                _cells[cellIndex].Add(entity);
            }
        }
        
        public IEnumerable<T> GetNearbyEntities(Vector2 position, float radius) {
            var nearbyEntities = new List<T>();
            int cellIndex = PositionToIndex(position);
            Vector2 cellSize = _spaceSize / _numCells;
            int cellsToCheck = Mathf.CeilToInt(radius / cellSize.magnitude);

            for (int i = -cellsToCheck; i <= cellsToCheck; i++) {
                for (int j = -cellsToCheck; j <= cellsToCheck; j++) {
                    int x = Mathf.Clamp(i + cellIndex % _numCells.x, 0, _numCells.x - 1);
                    int y = Mathf.Clamp(j + cellIndex / _numCells.x, 0, _numCells.y - 1);
                    int index = y * _numCells.x + x;

                    if (_cells[index] == null) continue;

                    foreach (T entity in _cells[index])
                        if (Vector2.Distance(position, entity.transform.position) <= radius)
                            nearbyEntities.Add(entity);
                }
            }

            return nearbyEntities;
        }
        
        public void Add(T entity) => _entities.Add(entity);
        public void Remove(T entity) => _entities.Remove(entity);

        private int PositionToIndex(Vector2 position) {
            Vector2 localPosition = position - _center + _spaceSize * 0.5f;

            int cellX = Mathf.Clamp(Mathf.FloorToInt(localPosition.x / (_spaceSize.x / _numCells.x)), 0, _numCells.x - 1);
            int cellY = Mathf.Clamp(Mathf.FloorToInt(localPosition.y / (_spaceSize.y / _numCells.y)), 0, _numCells.y - 1);

            return cellY * _numCells.x + cellX;
        }
    }
}