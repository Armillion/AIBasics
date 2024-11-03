using Shooter.FSM;

namespace Shooter.Agents.States {
    public abstract class BaseState : IState {
        protected Agent agent;
        
        public BaseState(Agent agent) => this.agent = agent;
        
        public abstract void OnEnter();
        public abstract void Update();
        public abstract void OnExit();
    }
}