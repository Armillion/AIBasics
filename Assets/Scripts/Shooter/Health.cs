using System;
using Gaskellgames;
using UnityEngine;

namespace Shooter {
    public class Health : MonoBehaviour {
        [field: SerializeField, UnityEngine.Min(float.Epsilon)]
        public float MaxHealth { get; private set; } = 100;
        
        [field: SerializeField, ReadOnly]
        public float CurrentHealth { get; private set; }
        
        public float NormalizedHealth => CurrentHealth / MaxHealth;

        private void Start() {
            CurrentHealth = MaxHealth;
        }
        
        public void Heal(float amount) => CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        public void TakeDamage(float amount) => CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
    }
}