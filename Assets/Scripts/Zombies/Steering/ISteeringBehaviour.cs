using DefaultNamespace;
using UnityEngine;

namespace Zombies.Steering {
    public interface ISteeringBehaviour {
        Vector2 CalculateSteering(IVehicle vehicle);
    }
}