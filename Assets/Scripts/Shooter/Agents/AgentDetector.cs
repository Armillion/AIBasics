using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace Shooter.Agents {
    public class AgentDetector : MonoBehaviour {
        [SerializeField, Self]
        private Agent _agent;
        
        [SerializeField, Min(0f)]
        private float _detectionRadius = 10f;
        
        public IReadOnlyCollection<Agent> HostileAgents => _hostileAgents;
        public bool HasHostileAgents => _hostileAgents.Count > 0;
        
        private readonly HashSet<Agent> _hostileAgents = new();

        private void OnValidate() => this.ValidateRefs();

        public void Tick() {
            foreach (Agent other in Agent.Agents) {
                if (other == _agent)
                    continue;
                
                Vector3 agentPosition = _agent.transform.position;
                Vector3 otherPosition = other.transform.position;
                bool hasLineOfSight = _agent.Arena.IsStraightPathTraversable(agentPosition, otherPosition);
                
                if (Vector3.Distance(agentPosition, otherPosition) > _detectionRadius || !hasLineOfSight) {
                    _hostileAgents.Remove(other);
                    continue;
                }
                
                if (_agent.Team.IsHostile(other.Team))
                    _hostileAgents.Add(other);
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}