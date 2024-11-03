using System;
using ImprovedTimers;
using Shooter.Environment;
using UnityEngine;

namespace Shooter.Agents {
    public class AgentManager : MonoBehaviour {
        public static AgentManager Instance { get; private set; }
        
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
        private AgentsConfig[] _agentsConfig = Array.Empty<AgentsConfig>();

        [field: SerializeField, Min(0.1f)]
        public float TickSpeed { get; private set; } = 0.5f;

        private FrequencyTimer _tickTimer;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            int ticksPerSecond = Mathf.RoundToInt(1f / TickSpeed);
            _tickTimer = new FrequencyTimer(ticksPerSecond);
            _tickTimer.OnTick += Tick;
            _tickTimer.Start();
            
            foreach (AgentsConfig config in _agentsConfig)
                for (var i = 0; i < config.count; i++)
                    _agentSpawner.SpawnAgent(_arena, config.team);
        }

        private static void Tick() {
            foreach (Agent agent in Agent.Agents)
                agent.Tick();
        }
    }
}