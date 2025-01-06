using Shooter.Agents;
using UnityEngine;

namespace Shooter.Pickups.PickupStrategies {
    [CreateAssetMenu(menuName = "Pickup Strategies/Restore Armor", fileName = "Restore Armor Strategy")]
    public class RestoreArmorStrategy : PickupStrategy {
        public override bool ApplyEffect(Agent agent, params int[] args) {
            if (agent.Health.CurrentArmor >= agent.Health.MaxArmor)
                return false;
            
            agent.Health.RestoreArmor(args[0]);
            return true;
        }
    }
}