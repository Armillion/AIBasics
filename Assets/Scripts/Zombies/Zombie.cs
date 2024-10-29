using System;
using Gaskellgames;
using UnityEngine;
using Zombies.Steering;

namespace Zombies {
    public class Zombie : MonoBehaviour, IVehicle {
        [Serializable]
        private struct SteeringBehaviourConfig {
            [SerializeField]
            private Component _component;
            
            public ISteeringBehaviour Behaviour => _component as ISteeringBehaviour;
            public float weight;
        }
        
        [field: SerializeField, UnityEngine.Min(0f)]
        public float MaxSpeed { get; private set; } = 2f;

        [SerializeField]
        private SteeringBehaviourConfig[] _steeringBehaviours;
        
        public Vector2 Position => transform.position;
        [field: SerializeField, ReadOnly] public Vector2 Velocity { get; private set; }
        
        private void Update() {
            Vector2 steering = Vector2.zero;
            
            foreach (SteeringBehaviourConfig steeringBehaviour in _steeringBehaviours)
                steering += steeringBehaviour.Behaviour.CalculateSteering(this) * steeringBehaviour.weight;
            
            Velocity += steering * Time.deltaTime;
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

            transform.position += (Vector3)Velocity * Time.deltaTime;

            if (Velocity.magnitude <= 0.1f) return;
            
            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}