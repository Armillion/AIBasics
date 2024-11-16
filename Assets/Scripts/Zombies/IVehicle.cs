using UnityEngine;

namespace Zombies {
    public interface IVehicle {
        float MaxSpeed { get; }
        Vector2 Position { get; }
        Vector2 Velocity { get; }
    }
}
