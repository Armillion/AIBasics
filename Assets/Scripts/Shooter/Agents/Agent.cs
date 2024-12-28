using KBCore.Refs;
using Physics;
using Shooter.Environment;
using UnityEngine;
using Utility.GizmosLegend;

namespace Shooter.Agents {
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SimpleCircleCollider))]
    public class Agent : MonoBehaviour {
        [SerializeField]
        private AgentConfig _agentConfig;
        
        [SerializeField, Self]
        private Health _health;

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

#if UNITY_EDITOR
        private void OnValidate() {
            this.ValidateRefs();
            SetupAgentSize();
        }
#endif

        public void Initialize(Arena arena, Team team) {
            _arena = arena;
            Team = team;
            SetupAgentSize();
        }

        public void TakeDamage(float damage) {
            _health.TakeDamage(damage);
            if (_health.CurrentHealth <= 0f) Destroy(gameObject);
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

        private void OnDrawGizmosSelected() {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            const float lineLength = 5f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, _angleAccuracy) * Vector2.up * lineLength);
            Gizmos.DrawLine(Vector3.zero, Quaternion.Euler(0, 0, -_angleAccuracy) * Vector2.up * lineLength);
            Gizmos.matrix = Matrix4x4.identity;
            
            GizmosLegend.AddLabel(this, "Accuracy", Color.red, GizmoType.Line);
        }
    }
}