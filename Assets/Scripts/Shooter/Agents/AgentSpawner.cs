using System;
using Shooter.Environment;
using UnityEditor;
using UnityEngine;
using Utility.GizmosLegend;
using GizmoType = Utility.GizmosLegend.GizmoType;

namespace Shooter.Agents {
    public class AgentSpawner : MonoBehaviour {
        private const int MAX_SPAWN_ATTEMPTS = 10;
        
        [Serializable]
        private struct SpawnConfig {
            [SerializeField]
            private Transform _spawn;
            public float radius;
            public Team team;
            public Vector3 Position => _spawn ? _spawn.position : Vector3.zero;
        }
        
        [SerializeField]
        private Agent _agentTemplate;
        
        [SerializeField]
        private SpawnConfig[] _spawnConfigs = Array.Empty<SpawnConfig>();

        private void Start() => _agentTemplate.gameObject.SetActive(false);

        public void SpawnAgent(Arena arena, Team team) {
            for (var i = 0; i < MAX_SPAWN_ATTEMPTS; i++) {
                Vector2 position = GetRandomSpawnPosition(team);
                if (!arena.TryGetValidCellPosition(ref position)) continue;
                
                Agent agent = Instantiate(_agentTemplate, position, Quaternion.identity);
                agent.gameObject.SetActive(true);
                agent.Initialize(arena, team);
                return;
            }
            
            Debug.LogWarning("Failed to find a valid cell for agent spawn.");
        }
        
        private Vector2 GetRandomSpawnPosition(Team team) {
            SpawnConfig config = GetSpawnConfig(team);
            Vector2 position = config.Position;
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * config.radius;
            return position + randomPoint;
        }
        
        private SpawnConfig GetSpawnConfig(Team team) {
            foreach (SpawnConfig config in _spawnConfigs)
                if (config.team == team)
                    return config;
            
            return default;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            foreach (SpawnConfig config in _spawnConfigs) {
                Gizmos.color = config.team.GetColor();
                Gizmos.DrawSphere(config.Position, config.radius);
                GizmosLegend.AddLabel(this, config.team.ToString(), config.team.GetColor(), GizmoType.Sphere);
            }
        }
#endif
    }
}