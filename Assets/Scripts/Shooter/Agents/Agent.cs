using System;
using System.Collections.Generic;
using KBCore.Refs;
using Physics;
using Shooter.Agents.States;
using Shooter.Environment;
using Shooter.FSM;
using Shooter.Pickups.PickupStrategies;
using Shooter.Weapons;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
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

        [field: SerializeField, Child]
        public Weapon Weapon { get; private set; }

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
            SetupAgentDetector();
            SetupStateMachine();
            
            AllAgents.Add(this);
            Health.onDeath.AddListener(DestroySelf);
        }
        
        public void Shoot(Agent target) {
            Vector3 direction = target.transform.position - transform.position;
            Weapon.Shoot(transform.position, direction);
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
        
        private void SetupAgentDetector() {
            if (!_agentConfig) {
                Debug.LogError("Agent missing AgentConfig", this);
                return;
            }

            _agentDetector = new AgentDetector(this, _agentConfig.VisionConeAngle);
            Health.onDamageTaken.AddListener(attacker => _agentDetector.AddAggressorAgents(attacker as Agent));
        }

        private void SetupStateMachine() {
            _stateMachine = new StateMachine();
            
            var wanderState = new WanderState(this, _arena, _agentConfig.RotationSpeed, _agentConfig.MoveSpeed, _agentConfig.WanderRadius);
            var attackState = new AttackState(this, _arena, _agentDetector, _agentConfig.RotationSpeed, _agentConfig.MoveSpeed);
            var collectHealthPickupState = new CollectPickupState<HealAgentStrategy>(this, _arena, _agentConfig.RotationSpeed, _agentConfig.MoveSpeed);
            var collectArmorPickupState = new CollectPickupState<RestoreArmorStrategy>(this, _arena, _agentConfig.RotationSpeed, _agentConfig.MoveSpeed);
            var collectAmmoPickupState = new CollectPickupState<RefillAmmoStrategy>(this, _arena, _agentConfig.RotationSpeed, _agentConfig.MoveSpeed);
            
            _stateMachine.AddTransition(attackState, collectHealthPickupState, new FuncPredicate(() => Health.CurrentTotalHitPoints < _agentConfig.HitPointsPanicThreshold));
            _stateMachine.AddTransition(attackState, collectAmmoPickupState, new FuncPredicate(() => Weapon.CurrentAmmo == 0));

            var hasDetectedAgentsPredicate = new FuncPredicate(() => _agentDetector.HasDetectedAgents);
            _stateMachine.AddTransition(wanderState, attackState, hasDetectedAgentsPredicate);
            _stateMachine.AddTransition(collectHealthPickupState, attackState, new FuncPredicate(() => hasDetectedAgentsPredicate.Evaluate() && Health.CurrentTotalHitPoints >= _agentConfig.HitPointsPanicThreshold));
            _stateMachine.AddTransition(collectAmmoPickupState, attackState, new FuncPredicate(() => hasDetectedAgentsPredicate.Evaluate() && Weapon.CurrentAmmo > 0));
            _stateMachine.AddTransition(collectArmorPickupState, attackState, hasDetectedAgentsPredicate);
            
            _stateMachine.AddTransition(collectHealthPickupState, attackState, new FuncPredicate(() => !collectHealthPickupState.IsAnyPickupAvailable));
            _stateMachine.AddTransition(collectAmmoPickupState, attackState, new FuncPredicate(() => !collectAmmoPickupState.IsAnyPickupAvailable));
            _stateMachine.AddTransition(collectArmorPickupState, attackState, new FuncPredicate(() => !collectArmorPickupState.IsAnyPickupAvailable));
            
            _stateMachine.AddTransition(wanderState, collectHealthPickupState, new FuncPredicate(() => Health.HealthNormalized < .95f && collectHealthPickupState.IsAnyPickupAvailable));
            _stateMachine.AddTransition(wanderState, collectAmmoPickupState, new FuncPredicate(() => Weapon.CurrentAmmo < Health.MaxHealth / (float)Weapon.Damage * 1.75f && collectAmmoPickupState.IsAnyPickupAvailable));
            _stateMachine.AddTransition(wanderState, collectArmorPickupState, new FuncPredicate(() => Health.ArmorNormalized < .8f && collectArmorPickupState.IsAnyPickupAvailable));
            
            _stateMachine.AddTransition(collectHealthPickupState, wanderState, new FuncPredicate(() => Health.HealthNormalized >= .95f));
            _stateMachine.AddTransition(collectAmmoPickupState, wanderState, new FuncPredicate(() => Weapon.CurrentAmmo >= Health.MaxHealth / (float)Weapon.Damage * 1.75f));
            _stateMachine.AddTransition(collectArmorPickupState, wanderState, new FuncPredicate(() => Health.ArmorNormalized >= .8f));
            
            _stateMachine.AddTransition(attackState, wanderState, new FuncPredicate(() => !_agentDetector.HasDetectedAgents));
            
            _stateMachine.SetState(wanderState);
        }

        private void DestroySelf() {
            AllAgents.Remove(this);
            Destroy(gameObject);
        }

        private void OnDrawGizmos() => Handles.Label(transform.position + Vector3.up * 1.5f, _stateMachine?.CurrentState);

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