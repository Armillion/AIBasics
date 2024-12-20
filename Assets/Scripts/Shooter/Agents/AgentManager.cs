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
        
        public bool IsGamePaused => !_tickTimer?.IsRunning ?? false;

        private FrequencyTimer _tickTimer;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        public static void Tick() {
            foreach (Agent agent in Agent.Agents)
                agent.Tick();
        }

        public void ResumeGame() => _tickTimer?.Resume();
        
        public void PauseGame() => _tickTimer?.Pause();

        private void Start() {
            SetupTickTimer();

            foreach (AgentsConfig config in _agentsConfig)
                for (var i = 0; i < config.count; i++)
                    _agentSpawner.SpawnAgent(_arena, config.team);
        }

        private void SetupTickTimer() {
            _tickTimer?.Stop();
            _tickTimer?.Dispose();
            
            int ticksPerSecond = Mathf.RoundToInt(1f / TickSpeed);
            _tickTimer = new FrequencyTimer(ticksPerSecond);
            _tickTimer.OnTick += Tick;
            _tickTimer.Start();
        }
    }
}