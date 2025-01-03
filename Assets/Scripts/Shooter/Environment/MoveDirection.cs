using System;

namespace Shooter.Environment {
    [Flags]
    public enum MoveDirection {
        None = 0,
        North = 1 << 0,
        NorthEast = 1 << 1,
        East = 1 << 2,
        SouthEast = 1 << 3,
        South = 1 << 4,
        SouthWest = 1 << 5,
        West = 1 << 6,
        NorthWest = 1 << 7,
        All = North | NorthEast | East | SouthEast | South | SouthWest | West | NorthWest
    }
    
    public static class MoveDirectionExtensions {
        private const MoveDirection NORTHERLY_DIRECTIONS = MoveDirection.North | MoveDirection.NorthEast | MoveDirection.NorthWest;
        private const MoveDirection EASTERLY_DIRECTIONS = MoveDirection.East | MoveDirection.NorthEast | MoveDirection.SouthEast;
        private const MoveDirection SOUTHERLY_DIRECTIONS = MoveDirection.South | MoveDirection.SouthEast | MoveDirection.SouthWest;
        private const MoveDirection WESTERLY_DIRECTIONS = MoveDirection.West | MoveDirection.NorthWest | MoveDirection.SouthWest;

        public static bool IsNortherly(this MoveDirection direction) => (direction & NORTHERLY_DIRECTIONS) != 0;
        public static bool IsEasterly(this MoveDirection direction) => (direction & EASTERLY_DIRECTIONS) != 0;
        public static bool IsSoutherly(this MoveDirection direction) => (direction & SOUTHERLY_DIRECTIONS) != 0;
        public static bool IsWesterly(this MoveDirection direction) => (direction & WESTERLY_DIRECTIONS) != 0;
        
        public static MoveDirection Opposite(this MoveDirection direction) {
            return direction switch {
                MoveDirection.North => MoveDirection.South,
                MoveDirection.NorthEast => MoveDirection.SouthWest,
                MoveDirection.East => MoveDirection.West,
                MoveDirection.SouthEast => MoveDirection.NorthWest,
                MoveDirection.South => MoveDirection.North,
                MoveDirection.SouthWest => MoveDirection.NorthEast,
                MoveDirection.West => MoveDirection.East,
                MoveDirection.NorthWest => MoveDirection.SouthEast,
                _ => MoveDirection.None
            };
        }
    }
}