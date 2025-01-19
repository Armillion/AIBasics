using Shooter.FSM;
using Shooter.Environment;

namespace Shooter.Agents.States {
    public abstract class AgentBaseState : IState {
        protected readonly Agent agent;
        protected readonly Arena arena;
        protected readonly float rotationSpeed;
        protected readonly float moveSpeed;
            
        public AgentBaseState(Agent agent, Arena arena, float rotationSpeed, float moveSpeed) {
            this.agent = agent;
            this.arena = arena;
            this.rotationSpeed = rotationSpeed;
            this.moveSpeed = moveSpeed;
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void OnExit() { }
    }
}