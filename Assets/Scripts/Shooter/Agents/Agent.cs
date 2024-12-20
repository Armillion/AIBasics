using System;
using System.Collections.Generic;
using KBCore.Refs;
using Physics;
using PrimeTween;
using Shooter.Agents.States;
using Shooter.Environment;
using Shooter.FSM;
using UnityEngine;

namespace Shooter.Agents {
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(AgentDetector))]
    [RequireComponent(typeof(SimpleCircleCollider))]
    public class Agent : MonoBehaviour {
        private static readonly List<Agent> _agents = new();

        public static IReadOnlyList<Agent> Agents => _agents;
        
        [SerializeField, Self]
        private Health _health;
        
        [SerializeField, Self]
        private AgentDetector _detector;
        
        [SerializeField, Child]
        private SpriteRenderer _spriteRenderer;

        [SerializeField, Min(0.1f)]
        private float _wanderRadius = 10f;

        [SerializeField, Min(0f)]
        private float _angleAccuracy = 2f;
        
        [field: SerializeField, Self]
        public SimpleCircleCollider Collider { get; private set; }

        public Team Team {
            get => _team;

            private set {
                _team = value;
                name = $"{Team} Agent";
                _spriteRenderer.color = _team.GetColor();
            }
        }

        public Arena Arena { get; private set; }
        
        private Team _team;
        private StateMachine _stateMachine;

#if UNITY_EDITOR
        private void OnValidate() => this.ValidateRefs();
#endif
        
        private void Awake() => _stateMachine = new StateMachine();
        private void OnEnable() => _agents.Add(this);
        private void OnDisable() => _agents.Remove(this);
        
        public void Initialize(Arena arena, Team team) {
            Arena = arena;
            Team = team;
            
            var wanderState = new WanderState(this, Arena, _wanderRadius);
            var attackState = new AttackState(this, _detector.HostileAgents, _angleAccuracy);
            
            _stateMachine.AddTransition(wanderState, attackState, new FuncPredicate(() => _detector.HasHostileAgents));
            _stateMachine.AddTransition(attackState, wanderState, new FuncPredicate(() => !_detector.HasHostileAgents));
            
            _stateMachine.SetState(wanderState);
        }
        
        public void Tick() {
            _detector.Tick();
            _stateMachine.Update();
        }

        public void MoveTo(Vector2 destination, Ease ease = Ease.Linear) {
            Tween.Position(transform, destination, AgentManager.Instance.TickSpeed, ease);
            Vector3 direction = destination - (Vector2) transform.position;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
            Tween.Rotation(transform, rotation, AgentManager.Instance.TickSpeed, ease);
        }
        
        public void TakeDamage(float damage) {
            Debug.Log("Ouchie!", gameObject);
            _health.TakeDamage(damage);
            if (_health.CurrentHealth <= 0f) Destroy(gameObject);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
   
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Vector3.zero, _wanderRadius);
            
            const float lineLength = 5f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, _angleAccuracy) * Vector2.up * lineLength);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, -_angleAccuracy) * Vector2.up * lineLength);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}