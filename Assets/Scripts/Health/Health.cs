using ImprovedTimers;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
    [field: SerializeField, Min(1)]
    public int MaxHealth { get; private set; } = 3;

    [field: SerializeField]
    public int CurrentHealth { get; private set; }
    
    [field: SerializeField, Min(1)]
    public int MaxArmor { get; private set; }

    [field: SerializeField]
    public int CurrentArmor { get; private set; }

    [field: SerializeField, Min(0f)]
    public float InvulnerabilityPeriod { get; private set; } = 0.2f;

    public UnityEvent onHealthChanged;
    public UnityEvent<object> onDamageTaken;
    public UnityEvent onDeath;
    
    public float HealthNormalized => (float) CurrentHealth / MaxHealth;
    public float ArmorNormalized => (float) CurrentArmor / MaxArmor;
    public int CurrentTotalHitPoints => CurrentHealth + CurrentArmor;
    public int TotalHitPointsMax => MaxHealth + MaxArmor;
    public float TotalHitPointsNormalized => (float) CurrentTotalHitPoints / TotalHitPointsMax;

    private CountdownTimer _invulnerabilityTimer; 
    
    private void Awake() {
        CurrentHealth = MaxHealth;
        _invulnerabilityTimer = new CountdownTimer(InvulnerabilityPeriod);
    }

    public void TakeDamage(int amount = 1, object dealer = null) {
        if (_invulnerabilityTimer.IsRunning || CurrentHealth <= 0)
            return;
        
        if (CurrentArmor > 0) {
            int armorDamage = Mathf.Min(amount, CurrentArmor);
            CurrentArmor -= armorDamage;
            amount -= armorDamage;
            onHealthChanged?.Invoke();
            onDamageTaken?.Invoke(dealer);
        }

        CurrentHealth -= Mathf.Min(amount, CurrentHealth);
        onHealthChanged?.Invoke();
        onDamageTaken?.Invoke(dealer);
    
        if (CurrentHealth <= 0)
            onDeath?.Invoke();
    
        if (InvulnerabilityPeriod > 0)
            _invulnerabilityTimer.Start();
    }
    
    public void Heal(int amount) {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        onHealthChanged?.Invoke();
    }

    public void RestoreArmor(int amount) {
        CurrentArmor = Mathf.Min(CurrentArmor + amount, MaxArmor);
        onHealthChanged?.Invoke();
    }
}
