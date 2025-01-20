using System;
using System.Collections.Generic;
using System.Linq;
using Physics;
using Shooter.Environment;
using UnityEngine;

namespace Shooter.Agents {
    [Serializable]
    public class AgentDetector {
        public IReadOnlyCollection<Agent> Agents => _agents.Concat(_aggressorAgents).ToArray();

        public Agent Closest {
            get {
                if (Agents.Count == 0) return null;
                
                Agent closestAgent = null;
                float closestDistance = float.MaxValue;
                
                foreach (Agent agent in Agents) {
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
        private readonly HashSet<Agent> _aggressorAgents = new();

        public List<Agent> detectedAgents;
        
        public AgentDetector(Agent agent, float visionConeAngle) {
            _agent = agent;
            _visionConeAngle = visionConeAngle;
        }
        
        public void Update() {
            var agentsToRemove = _aggressorAgents.Where(IsAgentVisible).ToList();

            foreach (Agent agent in agentsToRemove)
                _aggressorAgents.Remove(agent);

            agentsToRemove.Clear();
            
            foreach (Agent agent in Agent.AllAgents) {
                if (agent == _agent) continue;

                if (IsAgentVisible(agent))
                    _agents.Add(agent);
                else if (!IsAgentVisible(agent) && _agents.Contains(agent))
                    agentsToRemove.Add(agent);
            }
            
            foreach (Agent agent in agentsToRemove)
                _agents.Remove(agent);

            foreach (Agent agent in _agents)
                Debug.DrawLine(_agent.transform.position, agent.transform.position, Color.yellow);
            
            detectedAgents = Agents.ToList();
        }
        
        public void AddAggressorAgents(Agent aggressorAgents) {
            if (aggressorAgents)
                _aggressorAgents.Add(aggressorAgents);
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