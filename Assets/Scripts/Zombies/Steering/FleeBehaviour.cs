using UnityEngine;

namespace Zombies.Steering {
    public class FleeBehaviour : MonoBehaviour, ISteeringBehaviour {
        [SerializeField, Min(0f)]
        private float _panicDistance = 10f;
        
        public Vector2 target;
        
        public Vector2 CalculateSteering(IVehicle zombie) {
            if (Vector2.Distance(zombie.Position, target) > _panicDistance)
                return Vector2.zero;
            
            Vector2 desiredVelocity = (zombie.Position - target).normalized * zombie.MaxSpeed;
            return desiredVelocity - zombie.Velocity;
        }
    }
}