using System;
using UnityEngine;
using Zombies.Steering;

namespace Zombies {
    public class Zombie : MonoBehaviour, IVehicle {
        [field: SerializeField, Min(0f)]
        public float MaxSpeed { get; private set; } = 2f;

        [SerializeField]
        private MonoBehaviour[] _steeringBehavioursMonoBehaviours;
        
        public Vector2 Position => transform.position;
        public Vector2 Velocity { get; private set; }
        
        private ISteeringBehaviour[] _steeringBehaviours;

        private void OnValidate() {
            if (_steeringBehavioursMonoBehaviours == null) return;

            foreach (MonoBehaviour monoBehaviour in _steeringBehavioursMonoBehaviours)
                if (!monoBehaviour.TryGetComponent<ISteeringBehaviour>(out _))
                    Debug.LogError($"{monoBehaviour.GetType().Name} does not implement ISteeringBehaviour", monoBehaviour);
        }
        
        private void Start() {
            _steeringBehaviours = new ISteeringBehaviour[_steeringBehavioursMonoBehaviours.Length];
            
            for (var i = 0; i < _steeringBehavioursMonoBehaviours.Length; i++)
                _steeringBehaviours[i] = _steeringBehavioursMonoBehaviours[i].GetComponent<ISteeringBehaviour>();
        }

        private void Update() {
            Vector2 steering = Vector2.zero;
            
            foreach (ISteeringBehaviour steeringBehaviour in _steeringBehaviours)
                steering = steeringBehaviour.CalculateSteering(this);
            
            Velocity += steering * Time.deltaTime;
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

            transform.position += (Vector3)Velocity * Time.deltaTime;

            if (Velocity.magnitude <= 0.1f) return;
            
            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}