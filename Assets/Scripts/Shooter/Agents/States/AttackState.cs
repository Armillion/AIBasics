using UnityEngine;
using Shooter.Environment;

namespace Shooter.Agents.States {
    public class AttackState : AgentBaseState {
        private readonly AgentDetector _agentDetector;
        private Vector2 _destination;

        public AttackState(Agent agent, Arena arena, AgentDetector agentDetector, float rotationSpeed, float moveSpeed) : base(agent, arena, rotationSpeed, moveSpeed) {
            _agentDetector = agentDetector;
        }

        public override void OnEnter() {
            base.OnEnter();
            _destination = arena.GetNeighbouringCellPosition(agent.transform.position);
        }

        public override void Update() {
            base.Update();
            Strafe();
            Shoot();
        }

        private void Strafe() {
            if (Vector2.Distance(agent.transform.position, _destination) == 0f)
                _destination = arena.GetNeighbouringCellPosition(agent.transform.position);
            
            Vector3 agentPosition = agent.transform.position;
            agent.transform.position = Vector2.MoveTowards(agentPosition, _destination, moveSpeed * Time.deltaTime);
            Debug.DrawLine(agentPosition, _destination, new Color(1f, 0.5f, 1f), 0.1f);

            Vector3 targetPosition = _agentDetector.Closest.transform.position;
            Vector3 dirToTarget = targetPosition - agentPosition;
            var lookRotation = Quaternion.LookRotation(Vector3.forward, (Vector2)dirToTarget);
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        private void Shoot() {
            Vector2 target = _agentDetector.Closest.transform.position;
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, Quaternion.LookRotation(Vector3.forward, target - (Vector2) agent.transform.position), rotationSpeed * Time.deltaTime);
            
            if (Quaternion.Angle(agent.transform.rotation, Quaternion.LookRotation(Vector3.forward, target - (Vector2) agent.transform.position)) <= float.Epsilon)
                agent.Shoot(_agentDetector.Closest);
        }
    }
}