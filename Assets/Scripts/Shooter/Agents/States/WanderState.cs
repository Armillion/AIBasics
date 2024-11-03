using System.Collections.Generic;
using System.Linq;
using Shooter.Environment;
using UnityEngine;

namespace Shooter.Agents.States {
    public class WanderState : BaseState {
        private readonly Arena _arena;
        private readonly float _wanderRadius;
        private List<Vector2> _path = new();
        private int _index;

        public WanderState(Agent agent, Arena arena, float wanderRadius) : base(agent) {
            _arena = arena;
            _wanderRadius = wanderRadius;
        }

        public override void OnEnter() {
            FindNewWanderDestination();
        }

        public override void Update() {
            if ((_path == null || _index >= _path.Count - 1) && !FindNewWanderDestination())
                return;

            _index++;
            
            for (int i = _index; i < _path.Count - 1; i++)
                Debug.DrawLine(_path[i], _path[i + 1], Color.cyan, AgentManager.Instance.TickSpeed);
            
            agent.transform.position = _path![_index];
        }

        public override void OnExit() { }
     
        private bool FindNewWanderDestination() {
            Vector2 randomDestination = _arena.GetRandomPointInArena(agent.transform.position, _wanderRadius);
            _path = _arena.FindPath(agent.transform.position, randomDestination)?.ToList();

            if (_path == null || _path.Count <= 1) {
                Debug.LogWarning($"{agent} couldn't find path to wander destination", agent);
                return false;
            }

            _index = 0;
            agent.transform.position = _path[_index];

            return true;
        }
    }
}