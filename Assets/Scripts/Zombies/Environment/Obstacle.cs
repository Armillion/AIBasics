using System;
using UnityEngine;

namespace Zombies.Environment {
    public class Obstacle : MonoBehaviour {
        [field: SerializeField, Min(0f)]
        public float Radius { get; private set; } = 3f;

        private void OnValidate() {
            transform.localScale = new Vector3(Radius, Radius, 1f);
        }
    }
}