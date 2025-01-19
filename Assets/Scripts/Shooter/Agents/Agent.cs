using System.Collections.Generic;
using KBCore.Refs;
using Physics;
using Shooter.Agents.States;
using Shooter.Environment;
using Shooter.FSM;
using UnityEngine;
using Utility.DescriptiveGizmos;

namespace Shooter.Agents {
    [SelectionBase]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SimpleCircleCollider))]
    public class Agent : MonoBehaviour {
        public static List<Agent> AllAgents { get; } = new List<Agent>();
        
        [SerializeField]
        private AgentConfig _agentConfig;
        
        [field: SerializeField, Self]
        public Health Health { get; private set; }

        [SerializeField, Child]
        private SpriteRenderer _spriteRenderer;

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

        private Team _team;
        private Arena _arena;
        
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
            _stateMachine.AddAnyTransition(wanderState, new FuncPredicate(() => true));
            _stateMachine.SetState(wanderState);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            const float lineLength = 5f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, _angleAccuracy) * Vector2.up * lineLength);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, -_angleAccuracy) * Vector2.up * lineLength);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, _agentConfig.VisionConeAngle * 0.5f) * Vector2.up * 1000f);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, -_agentConfig.VisionConeAngle * 0.5f) * Vector2.up * 1000f);
            Gizmos.matrix = Matrix4x4.identity;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _agentConfig.WanderRadius);
            
#if UNITY_EDITOR
            GizmosLegend.AddLabel(this, "Accuracy", Color.red, GizmoType.Line);
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