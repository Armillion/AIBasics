using System;
using System.Collections.Generic;
using KBCore.Refs;
using Shooter.Agents.States;
using Shooter.Environment;
using Shooter.FSM;
using Shooter.UtilityAI.Considerations;
using UnityEngine;

namespace Shooter.Agents {
    public class Agent : MonoBehaviour {
        private static readonly List<Agent> _agents = new();

        public static IReadOnlyList<Agent> Agents => _agents;

        [SerializeField]
        private List<Consideration> _considerations;
        
        [SerializeField, Child]
        private SpriteRenderer _spriteRenderer;

        [SerializeField, Min(0.1f)]
        private float _wanderRadius = 10f;
        
        private StateMachine _stateMachine;
        
        private Arena _arena;
        private Team _team;

#if UNITY_EDITOR
        private void OnValidate() => this.ValidateRefs();
#endif
        
        private void Awake() => _stateMachine = new StateMachine();
        private void OnEnable() => _agents.Add(this);
        private void OnDisable() => _agents.Remove(this);
        
        public void Initialize(Arena arena, Team team) {
            _arena = arena;
            _team = team;
            _spriteRenderer.color = team.GetColor();
            name = $"{team} Agent";

            var wanderState = new WanderState(this, _arena, _wanderRadius);
            _stateMachine.AddAnyTransition(wanderState, new FuncPredicate(() => true));
            _stateMachine.SetState(wanderState);
        }
        
        public void Tick() {
            _stateMachine.Update();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _wanderRadius);
        }
    }
}