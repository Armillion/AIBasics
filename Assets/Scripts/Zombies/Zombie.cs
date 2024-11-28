using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using Zombies.Steering;

namespace Zombies {
    [SelectionBase]
    [SaveDuringPlay]
    public class Zombie : MonoBehaviour, IVehicle {
        [Serializable]
        private struct SteeringBehaviourConfig {
            [SerializeField]
            private Component _component;

            public float weight;

            public ISteeringBehaviour Behaviour => _component as ISteeringBehaviour;

            public void OnValidate(Component validator = null) {
                if (!_component || Behaviour != null) return;
                Debug.LogError($"{_component} does not implement {nameof(ISteeringBehaviour)}", validator);
                _component = null;
            }
        }

        private static readonly HashSet<Zombie> _zombies = new();
        public static int ZombieCount => _zombies.Count;

        [SerializeField]
        private PlayerController _player;

        [field: SerializeField, Min(0f)]
        public float MaxSpeed { get; private set; } = 2f;

        [field: SerializeField, Min(0f)]
        public float MaxSteerForce { get; private set; } = 5f;
        
        [SerializeField]
        private SteeringBehaviourConfig[] _defaultSteeringBehaviours;

        [SerializeField]
        private SteeringBehaviourConfig[] _chaseSteeringBehaviours;

        [Header("Grouping")]
        [SerializeField, Min(0f)]
        private float _groupingRadius = 2f;

        [SerializeField, Min(1)]
        private int _attackGroupSize = 5;

        public Vector2 Position => transform.position;
        public Vector2 Velocity { get; private set; }
        
        private bool _isChasingPlayer;

        private void OnValidate() {
            foreach (SteeringBehaviourConfig steeringBehaviour in _defaultSteeringBehaviours)
                steeringBehaviour.OnValidate(this);
        }

        private void OnEnable() => _zombies.Add(this);

        private void OnDisable() {
            if (_zombies.Contains(this))
                _zombies.Remove(this);
        }

        private void Update() {
            if (!_isChasingPlayer && IsAttackGroupAssembled())
                _isChasingPlayer = true;
            
            Vector2 steering = _isChasingPlayer ? CalculateSteering(_chaseSteeringBehaviours) : CalculateSteering(_defaultSteeringBehaviours);
            Steer(steering);
        }

        private bool IsAttackGroupAssembled() {
            int closeZombies = _zombies.Where(zombie => zombie != this).Count(
                zombie => Vector2.Distance(Position, zombie.Position) <= _groupingRadius
            );

            return closeZombies >= _attackGroupSize;
        }

        private void Steer(Vector2 steering) {
            Velocity += steering * Time.deltaTime;
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

            transform.position += (Vector3)Velocity * Time.deltaTime;
            if (Velocity.magnitude <= float.Epsilon) return;

            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private Vector2 CalculateSteering(SteeringBehaviourConfig[] steeringBehaviours) {
            Vector2 steering = Vector2.zero;
            Color[] debugColors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.blue };
            int i = 0;
            
            foreach (SteeringBehaviourConfig steeringBehaviour in steeringBehaviours) {
                Vector2 forceToAdd = steeringBehaviour.Behaviour.CalculateSteering(this) * steeringBehaviour.weight;
                if (!AccumulateForce(ref steering, forceToAdd, debugColors[i++])) break;
            }

            return steering;
        }

        private bool AccumulateForce(ref Vector2 accumulatedForce, Vector2 forceToAdd, Color debugColor) {
            float magnitudeSoFar = accumulatedForce.magnitude;
            float magnitudeRemaining = MaxSteerForce - magnitudeSoFar;

            if (magnitudeRemaining <= 0f) return false;

            float magnitudeToAdd = forceToAdd.magnitude;

            if (magnitudeToAdd < magnitudeRemaining)
                accumulatedForce += forceToAdd;
            else
                accumulatedForce += forceToAdd.normalized * magnitudeRemaining;
            
            Debug.DrawRay(transform.position, accumulatedForce, debugColor);

            return true;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _groupingRadius);
        }
    }
}