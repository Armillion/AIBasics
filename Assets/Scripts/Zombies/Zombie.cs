using System;
using Gaskellgames;
using KBCore.Refs;
using UnityEngine;
using Zombies.Steering;

namespace Zombies {
    public class Zombie : MonoBehaviour, IVehicle {
        [field: SerializeField, UnityEngine.Min(0f)]
        public float MaxSpeed { get; private set; } = 2f;

        [SerializeField]
        private Component[] _steeringBehaviourComponents;
        
        public Vector2 Position => transform.position;
        [field: SerializeField, ReadOnly] public Vector2 Velocity { get; private set; }
        
        private ISteeringBehaviour[] _steeringBehaviours;

        private void OnValidate() {
            if (_steeringBehaviourComponents == null) return;

            foreach (Component component in _steeringBehaviourComponents)
                if (component && component is not ISteeringBehaviour)
                    Debug.LogError($"{component.GetType().Name} does not implement ISteeringBehaviour", component);
        }
        
        private void Start() {
            _steeringBehaviours = new ISteeringBehaviour[_steeringBehaviourComponents.Length];
            
            for (var i = 0; i < _steeringBehaviourComponents.Length; i++)
                _steeringBehaviours[i] = _steeringBehaviourComponents[i] as ISteeringBehaviour;
        }

        private void Update() {
            Vector2 steering = Vector2.zero;
            
            foreach (ISteeringBehaviour steeringBehaviour in _steeringBehaviours)
                steering += steeringBehaviour.CalculateSteering(this);
            
            Velocity += steering * Time.deltaTime;
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

            transform.position += (Vector3)Velocity * Time.deltaTime;

            if (Velocity.magnitude <= 0.1f) return;
            
            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}