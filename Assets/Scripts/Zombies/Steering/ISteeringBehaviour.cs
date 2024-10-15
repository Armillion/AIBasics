using UnityEngine;

namespace Zombies.Steering {
    public interface ISteeringBehaviour {
        Vector2 CalculateSteering(Zombie zombie);
    }
}