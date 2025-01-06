using Shooter.Agents;
using UnityEngine;

namespace Shooter.Pickups.PickupStrategies {
    public abstract class PickupStrategy : ScriptableObject {
        public abstract bool ApplyEffect(Agent agent, params int[] args);
    }
}