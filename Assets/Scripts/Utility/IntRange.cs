using UnityEngine;

namespace Utility {
    [System.Serializable]
    public struct IntRange {
        public int min, max;

        public int RandomValueInRange => Random.Range(min, max + 1);

        public IntRange(int min, int max) {
            this.min = min;
            this.max = min > max ? min : max;
        }
    }
}
