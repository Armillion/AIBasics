using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Environment {
    [Serializable]
    public struct Cell : IEqualityComparer<Cell> {
        public Vector2 position;
        public MoveDirection TraversableDirections { get; set; }

        public bool Equals(Cell x, Cell y)
            => x.position.Equals(y.position) && x.TraversableDirections == y.TraversableDirections;
        
        public int GetHashCode(Cell obj) => HashCode.Combine(obj.position, (int)obj.TraversableDirections);
    }
}