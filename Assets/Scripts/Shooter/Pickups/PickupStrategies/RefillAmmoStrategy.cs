using Shooter.Agents;
using UnityEngine;

namespace Shooter.Pickups.PickupStrategies {
    [CreateAssetMenu(menuName = "Pickup Strategies/Refill Ammunition", fileName = "Refill Ammo Strategy")]
    public class RefillAmmoStrategy : PickupStrategy {
        public override bool ApplyEffect(Agent agent, params int[] args) {
            return true;
        }
    }
}