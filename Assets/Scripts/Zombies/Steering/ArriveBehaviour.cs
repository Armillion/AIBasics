using UnityEngine;

namespace Zombies.Steering {
    public class ArriveBehaviour : MonoBehaviour, ISteeringBehaviour {
        public Vector2 target;
        
        [SerializeField, Min(0f)]
        private float _deceleration = 0.3f;
        
        public Vector2 CalculateSteering(IVehicle vehicle) {
            Vector2 toTarget = target - (Vector2)transform.position;
            float distance = toTarget.magnitude;

            if (distance <= 0f) return Vector2.zero;
            
            float speed = distance / _deceleration;
            speed = Mathf.Min(speed, vehicle.MaxSpeed);
            Vector2 desiredVelocity = toTarget * speed / distance;
            return desiredVelocity - vehicle.Velocity;
        }
    }
}