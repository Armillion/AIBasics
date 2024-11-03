using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Environment {
    public static class ArenaHelper {
        public static Vector2 GetRandomPointInArena(this Arena arena, Vector2 position, float radius, int maxAttempts = 10) {
            for (var i = 0; i < maxAttempts; i++) {
                Vector2 randomPoint = position + Random.insideUnitCircle * radius;
                if (!arena.TryGetValidCellPosition(ref randomPoint)) continue;
                return randomPoint;
            }
            
            Debug.LogWarning("Failed to find a valid cell for agent spawn.");
            return Vector2.zero;
        }

        public static IEnumerable<Vector2> FindPath(this Arena arena, Vector2 startPosition, Vector2 destination) {
            if (!arena.TryGetCellIndex(startPosition, out int startIndex))
                return null;
            
            if (!arena.TryGetCellIndex(destination, out int endIndex))
                return null;

            return Pathfinding.AStar(arena.Grid, startIndex, endIndex, arena.XCellCount);
        }
    }
}