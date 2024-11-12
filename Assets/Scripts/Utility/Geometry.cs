using UnityEngine;

namespace Utility {
    public static class Geometry {
        public static bool IsPolygonClockwise(Vector2[] polygon) {
            float signedArea = CalculateSignedArea(polygon);
            return signedArea < 0;
        }

        private static float CalculateSignedArea(Vector2[] polygon) {
            int n = polygon.Length;
            var area = 0f;

            for (var i = 0; i < n; i++) {
                Vector2 current = polygon[i];
                Vector2 next = polygon[(i + 1) % n];

                area += current.x * next.y - next.x * current.y;
            }

            return area * 0.5f;
        }
        
        public static bool LinesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
            float denominator = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);
            if (denominator == 0) return false;

            float t = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / denominator;
            float u = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / denominator;
            return t is >= 0 and <= 1 && u is >= 0 and <= 1;
        }
        
        public static bool LinesIntersect(Vector2 a, Vector2 b, Vector2[] polygon, bool isClosedShape) {
            for (var i = 0; i < polygon.Length - 1; i++) {
                Vector2 c = polygon[i];
                Vector2 d = polygon[i + 1];
                if (LinesIntersect(a, b, c, d)) return true;
            }

            return isClosedShape && LinesIntersect(a, b, polygon[^1], polygon[0]);
        }

        public static Vector2 Rotate(Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static Vector3 Homo(Vector2 pt) => new Vector3(pt.x, pt.y, 1f);

        public static Vector2 intersectPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector3 cc = Vector3.Cross(Vector3.Cross(Homo(a1), Homo(a2)), Vector3.Cross(Homo(b1), Homo(b2)));
            return ((Vector2)cc) / cc.z;
        }
    }
}