using System;
using UnityEngine;
using Zombies.Steering;

namespace Zombies {
    public class Zombie : MonoBehaviour {
        [field: SerializeField, Min(0f)]
        public float MaxSpeed { get; private set; } = 2f;

        private ISteeringBehaviour[] _steeringBehaviours;
        
        public Vector2 Velocity { get; private set; }

        private void Start() {
            _steeringBehaviours = GetComponents<ISteeringBehaviour>();
        }

        private void Update() {
            Vector2 steering = Vector2.zero;
            
            foreach (ISteeringBehaviour steeringBehaviour in _steeringBehaviours)
                steering = steeringBehaviour.CalculateSteering(this);
            
            // Apply steering force to velocity
            Velocity += steering * Time.deltaTime;
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

            // Move the agent based on the velocity
            transform.position += (Vector3)Velocity * Time.deltaTime;

            // Rotate the object to face its velocity direction (optional)
            if (Velocity.magnitude > 0.1f) {
                float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}