using System;
using UnityEngine;

namespace Zombies.Environment {
    [Serializable]
    public struct Polygon {
        public Vector2[] vertices;

        public Vector2 this[int i] => vertices[i];

        public static implicit operator Vector2[](Polygon d) => d.vertices;
        public static implicit operator Polygon(Vector2[] d) => new() { vertices = d };
        public Polygon(Vector2[] vertices) => this.vertices = vertices;
    }
}