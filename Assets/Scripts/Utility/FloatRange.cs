using UnityEngine;

namespace Utility {
    [System.Serializable]
    public struct FloatRange {
        public float RandomValueInRange => Random.Range(min, max);

        public float Range => max - min;

        public float min, max;

        public FloatRange(float min, float max) {
            this.min = min;
            this.max = max;
        }
    }
}
