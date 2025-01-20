using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Shooter.Environment;

namespace Shooter.Agents.States {
    public class WanderState : AgentBaseState {
        private readonly float _wanderRadius;
        private Vector2 _destination;
        private List<Vector2> _path;
        private int _pathIndex;

        public WanderState(Agent agent, Arena arena, float rotationSpeed, float moveSpeed, float wanderRadius) : base(agent, arena, rotationSpeed, moveSpeed) {
            _wanderRadius = wanderRadius;
        }
        
        public override void Update() {
            if ((_path == null || _pathIndex >= _path.Count - 1) && !FindNewWanderDestination())
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
        
        private bool FindNewWanderDestination() {
            Vector2 randomDestination = arena.GetRandomPointInsideArena(agent.transform.position, _wanderRadius);
            _path = arena.FindPath(agent.transform.position, randomDestination)?.ToList();

            if (_path == null || _path.Count <= 1) {
                Debug.LogWarning($"{agent} couldn't find path to wander destination", agent);
                return false;
            }

            _pathIndex = 0;
            agent.transform.position = _path[_pathIndex];

            return true;
        }
    }
}