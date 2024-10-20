using System;

namespace Shooter.FSM {
    public class FuncPredicate : IPredicate {
        private readonly Func<bool> _func;
        
        public FuncPredicate(Func<bool> func) {
            this._func = func;
        }
        
        public bool Evaluate() => _func.Invoke();
    }
}