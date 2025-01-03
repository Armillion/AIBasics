using System;
using Shooter.Environment;
using UnityEngine;
using UnityUtils;

namespace Shooter.Agents {
    public class AgentManager : Singleton<AgentManager> {
        [Serializable]
        private struct AgentsConfig {
            public Team team;
            public int count;
        }
        
        [SerializeField]
        private AgentSpawner _agentSpawner;
        
        [SerializeField]
        private Arena _arena;

        [SerializeField]
        [Tooltip("Setting agent to None team will result in FFA behaviour.")]
        private AgentsConfig[] _agentsConfig = Array.Empty<AgentsConfig>();
        
        private void Start() {
            foreach (AgentsConfig config in _agentsConfig)
                for (var i = 0; i < config.count; i++)
                    _agentSpawner.SpawnAgent(_arena, config.team);
        }
    }
}