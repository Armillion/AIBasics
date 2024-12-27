using UnityEngine;

namespace Shooter.Agents {
    [CreateAssetMenu(menuName = "Create AgentConfig", fileName = "AgentConfig", order = 0)]
    public class AgentConfig : ScriptableObject {
        [field: SerializeField, Min(float.Epsilon)]
        public float Radius { get; private set; } = 0.75f;
    }
}