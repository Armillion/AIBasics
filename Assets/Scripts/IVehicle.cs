using UnityEngine;

namespace DefaultNamespace {
    public interface IVehicle {
        float MaxSpeed { get; }
        Vector2 Position { get; }
        Vector2 Velocity { get; }
    }
}