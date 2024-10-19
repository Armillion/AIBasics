using System;
using UnityEngine;

namespace Shooter.Environment {
    [Serializable]
    public struct Cell {
        public Vector2 position;
        public MoveDirection traversableDirections;
    }
}