using System;
using UnityEngine;

namespace Environment {
    [Serializable]
    public struct Cell {
        public Vector2 position;
        public MoveDirection traversableDirections;
    }
}