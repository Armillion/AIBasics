using UnityEngine;

namespace Shooter.Weapons {
    [CreateAssetMenu(menuName = "Create WeaponConfig", fileName = "WeaponConfig", order = 0)]
    public class WeaponConfig : ScriptableObject {
        [field: SerializeField, Min(0)]
        public float FireRate { get; private set; } = 1f;
        
        [field: SerializeField, Min(0)]
        public int Damage { get; private set; } = 15;
        
        [field: SerializeField, Range(0f, 180f)]
        public float AngleAccuracy { get; private set; } = 2f;
        
        [field: SerializeField, Min(0)]
        public int MaxAmmo { get; private set; } = 60;
    }
}