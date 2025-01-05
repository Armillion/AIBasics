using System;
using Shooter.Environment;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shooter {
    public static class Pathfinding {
        public static IEnumerable<Vector2> AStar(Cell[] grid, int startIndex, int endIndex, int gridWidth) {
            var openSet = new HashSet<int> { startIndex };
            var cameFrom = new Dictionary<int, int>();
            var gScore = new Dictionary<int, float>();
            var fScore = new Dictionary<int, float>();

            for (var i = 0; i < grid.Length; i++) {
                gScore.Add(i, Mathf.Infinity);
                fScore.Add(i, Mathf.Infinity);
            }

            gScore[startIndex] = 0f;
            fScore[startIndex] = Distance(startIndex, endIndex, gridWidth);

            while (openSet.Count > 0) {
                int currentIndex = openSet.OrderBy(node => fScore[node]).First();

                if (currentIndex.Equals(endIndex))
                    return ReconstructPath(cameFrom, currentIndex).Select(index => grid[index].position);

                openSet.Remove(currentIndex);
                List<int> neighbours = GetTraversableNeighbours(grid, currentIndex, gridWidth);

                foreach (int neighbourIndex in neighbours) {
                    float tentativeGScore = gScore[currentIndex] + Distance(currentIndex, neighbourIndex, gridWidth);
                    if (!gScore.ContainsKey(neighbourIndex) || !(tentativeGScore < gScore[neighbourIndex])) continue;
                    
                    cameFrom[neighbourIndex] = currentIndex;
                    gScore[neighbourIndex] = tentativeGScore;
                    fScore[neighbourIndex] = gScore[neighbourIndex] + Distance(neighbourIndex, endIndex, gridWidth);

                    openSet.Add(neighbourIndex);
                }
            }

            return null;
        }

        private static List<int> ReconstructPath(Dictionary<int, int> cameFrom, int currentIndex) {
            List<int> totalPath = new();
            totalPath.Insert(0, currentIndex);

            while (cameFrom.ContainsKey(currentIndex)) {
                currentIndex = cameFrom[currentIndex];
                totalPath.Insert(0, currentIndex);
            }

            return totalPath;
        }

        private static float Distance(int fromIndex, int toIndex, int gridWidth) {
            Vector2Int from = GetCellIndex(fromIndex, gridWidth);
            Vector2Int to = GetCellIndex(toIndex, gridWidth);
            return Vector2Int.Distance(from, to);
        }

        private static Vector2Int GetCellIndex(int index, int gridWidth) {
            int x = index % gridWidth;
            int y = index / gridWidth;
            return new Vector2Int(x, y);
        }

        private static List<int> GetTraversableNeighbours(Cell[] grid, int index, int gridWidth) {
            MoveDirection traversableDirections = grid[index].TraversableDirections;
            List<int> neighbours = new(8);
            
            if (traversableDirections.HasFlag(MoveDirection.North))
                neighbours.Add(index + gridWidth);
            
            if (traversableDirections.HasFlag(MoveDirection.NorthEast))
                neighbours.Add(index + gridWidth + 1);
            
            if (traversableDirections.HasFlag(MoveDirection.East))
                neighbours.Add(index + 1);

            if (traversableDirections.HasFlag(MoveDirection.SouthEast))
                neighbours.Add(index - gridWidth + 1);
            
            if (traversableDirections.HasFlag(MoveDirection.South))
                neighbours.Add(index - gridWidth);
            
            if (traversableDirections.HasFlag(MoveDirection.SouthWest))
                neighbours.Add(index - gridWidth - 1);
            
            if (traversableDirections.HasFlag(MoveDirection.West))
                neighbours.Add(index - 1);
            
            if (traversableDirections.HasFlag(MoveDirection.NorthWest))
                neighbours.Add(index + gridWidth - 1);
            
            return neighbours;
        }
    }
}