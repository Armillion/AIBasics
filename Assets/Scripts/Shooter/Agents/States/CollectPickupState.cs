using System.Collections.Generic;
using System.Linq;
using Shooter.Environment;
using Shooter.Pickups;
using Shooter.Pickups.PickupStrategies;
using UnityEngine;

namespace Shooter.Agents.States {
    public class CollectPickupState<T> : AgentBaseState where T : PickupStrategy {
        public bool IsAnyPickupAvailable => _pickupsCache.Any(pickup => pickup.Enabled);
        
        private readonly List<Pickup> _pickupsCache;
        private List<Vector2> _path = new();
        private int _pathIndex;

        public CollectPickupState(Agent agent, Arena arena, float rotationSpeed, float moveSpeed) : base(agent, arena, rotationSpeed, moveSpeed) {
            _pickupsCache = Pickup.AllPickups.Where(p => p.Strategy is T).ToList();
        }

        public override void OnEnter() {
            base.OnEnter();
            _path.Clear();
        }

        public override void Update() {
            base.Update();
            
            if ((_path == null || _pathIndex >= _path.Count) && !FindPathToClosestPickup())
                return;
        
            Vector2 destination = _path![_pathIndex];
            Vector3 agentPosition = agent.transform.position;
            
            if (Vector2.Distance(agentPosition, destination) <= float.Epsilon)
                _pathIndex++;
            
            for (int i = _pathIndex; i < _path!.Count - 1; i++)
                Debug.DrawLine(_path[i], _path[i + 1], Color.cyan, 0.1f);

            agent.transform.position = Vector2.MoveTowards(agentPosition, destination, moveSpeed * Time.deltaTime);
            
            var lookRotation = Quaternion.LookRotation(Vector3.forward, destination - (Vector2)agentPosition);
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
        
        private bool FindPathToClosestPickup() {
            Pickup closestPickup = GetClosestPickup();
            if (!closestPickup) return false;
            _path = arena.FindPath(agent.transform.position, closestPickup.transform.position)?.ToList();
            
            if (_path == null || _path.Count <= 1) {
                Debug.LogWarning($"{agent} couldn't find path to pickup", agent);
                return false;
            }

            _pathIndex = 0;
            agent.transform.position = _path[_pathIndex];
            
            return true;
        }
        
        private Pickup GetClosestPickup() {
            Pickup closesPickup = null;
            float closestDistance = float.MaxValue;
            
            foreach (Pickup pickup in _pickupsCache) {
                if (!pickup.Enabled) continue;
                float distance = Vector2.Distance(agent.transform.position, pickup.transform.position);
                
                if (distance < closestDistance) {
                    closesPickup = pickup;
                    closestDistance = distance;
                }
            }
            
            return closesPickup;
        }
    }
}