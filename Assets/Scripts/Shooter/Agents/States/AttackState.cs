using System.Collections.Generic;
using Physics;
using UnityEngine;
using Utility;

namespace Shooter.Agents.States {
    public class AttackState : BaseState {
        private readonly IReadOnlyCollection<Agent> _hostileAgents;
        private Agent _target;
        private readonly float _accuracy;

        public AttackState(Agent agent, IReadOnlyCollection<Agent> hostileAgents, float accuracy) : base(agent) {
             _hostileAgents = hostileAgents;
             _accuracy = accuracy;
        }
        
        public override void OnEnter() { }

        public override void Update() {
            Vector3 agentPos = agent.transform.position;
            
            if (!_target) {
                _target = GetTarget();
                Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, _target.transform.position - agentPos);
                agent.transform.rotation = lookRotation;
            }
            
            Vector3 direction = _target.transform.position - agentPos;
            direction = Quaternion.Euler(0, 0, Random.Range(-_accuracy, _accuracy)) * direction;
            
            Debug.DrawRay(agentPos, direction, Color.red, AgentManager.Instance.TickSpeed);

            if (!SimplePhysics2D.Raycast(agentPos, direction, out SimpleRaycastHit2D hit, agent.Collider)) return;
            if (!hit.transform || !hit.transform.CompareTag("Agent")) return;
            
            var target = hit.transform.GetComponent<Agent>();
            Debug.Log($"{agent.name} hit {target.name}!", target);
            target?.TakeDamage(10f);
        }

        public override void OnExit() { }
        
        private Agent GetTarget() {
            Agent target = null;
            var minDistance = float.MaxValue;
            
            foreach (Agent other in _hostileAgents) {
                float distance = Vector3.Distance(agent.transform.position, other.transform.position);
                if (distance >= minDistance) continue;
                minDistance = distance;
                target = other;
            }
            
            return target;
        }
    }
}