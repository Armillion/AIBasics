using System;
using System.Collections.Generic;
using System.Linq;
using Physics;
using Shooter.Environment;
using UnityEngine;

namespace Shooter.Agents {
    public class AgentDetector {
        public bool HasDetectedAgents => _agents.Count > 0 || _aggressorAgents.Count > 0;
        
        public Agent Closest {
            get {
                if (!HasDetectedAgents) return null;

                Agent closestAgent = null;
                float closestDistance = float.MaxValue;

                IEnumerable<Agent> allAgents = _agents.Concat(_aggressorAgents);
                
                foreach (Agent agent in allAgents) {
                    float distance = Vector2.Distance(_agent.transform.position, agent.transform.position);

                    if (distance < closestDistance) {
                        closestAgent = agent;
                        closestDistance = distance;
                    }
                }

                return closestAgent;
            }
        }
        
        private readonly Agent _agent;
        private readonly Arena _arena;
        private readonly HashSet<Agent> _agents = new();
        private readonly float _visionConeAngle;
        private readonly HashSet<Agent> _aggressorAgents = new(); // Agents that shot this agent, but are not in the cone of vision

        public AgentDetector(Agent agent, float visionConeAngle) {
            _agent = agent;
            _visionConeAngle = visionConeAngle;
        }
        
        public void Update() {
            RemoveVisibleAggressorAgents();
            var agentsToRemove = _agents.Where(agent => !agent).ToList();
            
            foreach (Agent agent in Agent.AllAgents) {
                if (agent == _agent) continue;

                if (!agent) {
                    agentsToRemove.Add(agent);
                    continue;
                }

                if (IsAgentVisible(agent))
                    _agents.Add(agent);
                else if (!IsAgentVisible(agent) && _agents.Contains(agent))
                    agentsToRemove.Add(agent);
            }

            foreach (Agent agent in agentsToRemove)
                _agents.Remove(agent);

            foreach (Agent agent in _agents)
                Debug.DrawLine(_agent.transform.position, agent.transform.position, Color.yellow);
        }
        
        public void AddAggressorAgents(Agent aggressorAgents) {
            if (aggressorAgents)
                _aggressorAgents.Add(aggressorAgents);
        }
        
        private void RemoveVisibleAggressorAgents() { 
            var agentsToRemove = _aggressorAgents.Where(IsAgentVisible).ToList();

            foreach (Agent agent in agentsToRemove)
                _aggressorAgents.Remove(agent);
        }

        private bool IsAgentVisible(Agent agent) {
            Vector2 direction = agent.transform.position - _agent.transform.position;
            float angle = Vector2.Angle(_agent.transform.up, direction);
            
            if (angle > _visionConeAngle * 0.5f) return false;
            if (!SimplePhysics2D.Raycast(_agent.transform.position, direction, out SimpleRaycastHit2D hit, _agent.Collider)) return false;
            if (!hit.transform || !hit.transform.TryGetComponent(out Agent hitAgent)) return false;
            
            return hitAgent == agent;
        }
    }
}