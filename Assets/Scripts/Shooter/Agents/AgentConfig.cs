using UnityEngine;

namespace Shooter.Agents {
    [CreateAssetMenu(menuName = "Create AgentConfig", fileName = "AgentConfig", order = 0)]
    public class AgentConfig : ScriptableObject {
        [field: SerializeField, Min(float.Epsilon)]
        public float Radius { get; private set; } = 0.75f;
        
        [field: SerializeField, Min(0f)]
        public float WanderRadius { get; private set; } = 5f;
        
        [field: SerializeField, Min(0f)]
        public float MoveSpeed { get; private set; } = 5f;
        
        [field: SerializeField, Min(0f)]
        public float RotationSpeed { get; private set; } = 180f;
        
        [field: SerializeField, Min(0f)]
        public float VisionConeAngle { get; private set; } = 45f;
    }
}