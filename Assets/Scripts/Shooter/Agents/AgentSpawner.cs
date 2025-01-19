using System;
using KBCore.Refs;
using Physics;
using Shooter.Environment;
using UnityEditor;
using UnityEngine;
using Utility.DescriptiveGizmos;
using GizmoType = Utility.DescriptiveGizmos.GizmoType;

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
                int cellIndex = arena.SnapPositionToCell(ref position);
                
                if (cellIndex == -1) continue;
                
                Agent agent = Instantiate(_agentTemplate, position, Quaternion.identity);
                agent.gameObject.SetActive(true);
                agent.Initialize(arena, cellIndex, team);
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
                if (config.team == team && !IsSpawnerOccupied(config))
                    return config;
            
            return default;
        }
        
        private static bool IsSpawnerOccupied(SpawnConfig config)
            => SimplePhysics2D.OverlapCircle(config.Position, config.radius);

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            foreach (SpawnConfig config in _spawnConfigs) {
                Gizmos.color = config.team.GetColor();
                Gizmos.DrawWireSphere(config.Position, config.radius);
                GizmosLegend.AddLabel(this, $"{config.team.ToString()} Team Spawner", config.team.GetColor(), GizmoType.Sphere);
            }
        }

        private void OnDisable() => GizmosLegend.Unregister(this);
#endif
    }
}