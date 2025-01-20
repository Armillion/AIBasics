using Shooter.Agents;
using UnityEngine;

namespace Shooter.Pickups.PickupStrategies {
    [CreateAssetMenu(menuName = "Pickup Strategies/Refill Ammunition", fileName = "Refill Ammo Strategy")]
    public class RefillAmmoStrategy : PickupStrategy {
        public override bool ApplyEffect(Agent agent, params int[] args) {
            if (agent.Weapon.CurrentAmmo >= agent.Weapon.MaxAmmo) return false;
            agent.Weapon.CurrentAmmo += args[0];
            return true;
        }
    }
}