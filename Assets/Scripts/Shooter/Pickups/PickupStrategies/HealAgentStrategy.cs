using Shooter.Agents;
using UnityEngine;

namespace Shooter.Pickups.PickupStrategies {
    [CreateAssetMenu(menuName = "Pickup Strategies/Heal Agent", fileName = "Heal Agent Strategy")]
    public class HealAgentStrategy : PickupStrategy {
        public override bool ApplyEffect(Agent agent, params int[] args) {
            if (agent.Health.CurrentHealth >= agent.Health.MaxHealth)
                return false;
            
            agent.Health.Heal(args[0]);
            return true;
        }
    }
}