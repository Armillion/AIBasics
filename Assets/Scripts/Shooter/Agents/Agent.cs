using System.Collections.Generic;
using KBCore.Refs;
using Physics;
using Shooter.Agents.States;
using Shooter.Environment;
using Shooter.FSM;
using Shooter.Weapons;
using UnityEditor;
using UnityEngine;
using Utility.DescriptiveGizmos;
using GizmoType = Utility.DescriptiveGizmos.GizmoType;

namespace Shooter.Agents {
    [SelectionBase]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SimpleCircleCollider))]
    public class Agent : MonoBehaviour {
        public static List<Agent> AllAgents { get; } = new();
        
        [SerializeField]
        private AgentConfig _agentConfig;
        
        [field: SerializeField, Self]
        public Health Health { get; private set; }
        
        [SerializeField, Child]
        private Weapon _weapon;
        
        [SerializeField, Child]
        private SpriteRenderer _spriteRenderer;
        
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

        private Team _team;
        private Arena _arena;
        
        [SerializeField]
        private AgentDetector _agentDetector;
        private StateMachine _stateMachine;

#if UNITY_EDITOR
        private void OnValidate() {
            this.ValidateRefs();
            SetupAgentSize();
        }
#endif

        private void Update() {
            _agentDetector?.Update();
            _stateMachine?.Update();
        }

        public void Initialize(Arena arena, Team team) {
            _arena = arena;
            Team = team;
            
            SetupAgentSize();
            _agentDetector = new AgentDetector(this, _agentConfig.VisionConeAngle);
            SetupStateMachine();
            
            AllAgents.Add(this);
        }
        
        public void Shoot(Agent target) {
            Vector3 direction = target.transform.position - transform.position;
            _weapon.Shoot(transform.position, direction, Collider);
        }

        private void SetupAgentSize() {
            if (!_agentConfig) {
                Debug.LogError("Agent missing AgentConfig", this);
                return;
            }

            Collider.radius = _agentConfig.Radius;

            if (_spriteRenderer)
                _spriteRenderer.transform.localScale = Vector3.one * _agentConfig.Radius * 2f;
        }

        private void SetupStateMachine() {
            _stateMachine = new StateMachine();
            
            var wanderState = new WanderState(this, _arena, _agentConfig.RotationSpeed, _agentConfig.MoveSpeed, _agentConfig.WanderRadius);
            var attackState = new AttackState(this, _arena, _agentDetector, _agentConfig.RotationSpeed, _agentConfig.MoveSpeed);
            
            _stateMachine.AddTransition(wanderState, attackState, new FuncPredicate(() => _agentDetector.Agents.Count > 0));
            _stateMachine.AddTransition(attackState, wanderState, new FuncPredicate(() => _agentDetector.Agents.Count == 0));
            
            _stateMachine.SetState(wanderState);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, _agentConfig.VisionConeAngle * 0.5f) * Vector2.up * 1000f);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, -_agentConfig.VisionConeAngle * 0.5f) * Vector2.up * 1000f);
            
            Handles.Label(transform.position + Vector3.up * 1.5f, _stateMachine?.CurrentState);
            
            Gizmos.matrix = Matrix4x4.identity;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _agentConfig.WanderRadius);
            
#if UNITY_EDITOR
            GizmosLegend.AddLabel(this, "Vision Cone", Color.yellow, GizmoType.Line);
            GizmosLegend.AddLabel(this, "Wander Radius", Color.cyan, GizmoType.Sphere);
#endif
        }

        private void OnDestroy() {
            AllAgents.Remove(this);
#if UNITY_EDITOR
            GizmosLegend.Unregister(this);
#endif
        }
    }
}