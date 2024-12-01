using Gaskellgames;
using ImprovedTimers;
using UnityEngine;
using UnityEngine.Events;

namespace Health {
    public class Health : MonoBehaviour {
        [field: SerializeField, UnityEngine.Min(1)]
        public int MaxHealth { get; private set; } = 3;

        [field: SerializeField, ReadOnly]
        public int CurrentHeath { get; private set; }

        [field: SerializeField, UnityEngine.Min(0f)]
        public float InvulnerabilityPeriod { get; private set; } = 0.2f;

        public UnityEvent onDamageTaken;
        public UnityEvent onDeath;
    
        private CountdownTimer _invulnerabilityTimer; 
        
        private void Awake() {
            CurrentHeath = MaxHealth;
            _invulnerabilityTimer = new CountdownTimer(InvulnerabilityPeriod);
        }

        public void TakeDamage() {
            if (_invulnerabilityTimer.IsRunning || CurrentHeath <= 0)
                return;

            CurrentHeath--;
            onDamageTaken?.Invoke();
        
            if (CurrentHeath <= 0)
                onDeath?.Invoke();
        
            _invulnerabilityTimer.Start();
        }
    }
}
