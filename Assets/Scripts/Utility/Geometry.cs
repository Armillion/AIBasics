using System;
using UnityEngine;

namespace Utility {
    public static class Geometry {
        public static bool IsPointInPolygon(Vector2 point, Vector2[][] polygons) {
            var windingNumber = 0;

            foreach (Vector2[] polygon in polygons)
                windingNumber += CalculateWindingNumber(point, polygon);

            return windingNumber != 0;
        }

        public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
            => CalculateWindingNumber(point, polygon) != 0;

        private static int CalculateWindingNumber(Vector2 point, Vector2[] polygon) {
            var windingNumber = 0;
            int numPoints = polygon.Length;

            for (int i = 0, j = numPoints - 1; i < numPoints; j = i++) {
                Vector2 vertex1 = polygon[i];
                Vector2 vertex2 = polygon[j];

                if (vertex1.y <= point.y) {
                    if (vertex2.y > point.y && IsLeft(vertex1, vertex2, point) > 0)
                        windingNumber++;
                } else {
                    if (vertex2.y <= point.y && IsLeft(vertex1, vertex2, point) < 0)
                        windingNumber--;
                }
            }

            return windingNumber;
        }

        private static float IsLeft(Vector2 v1, Vector2 v2, Vector2 point)
            => (v2.x - v1.x) * (point.y - v1.y) - (point.x - v1.x) * (v2.y - v1.y);

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

        public static bool LinesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 point) {
            point = Vector2.zero;
            float denominator = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);

            if (denominator == 0) return false;

            float t = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / denominator;
            float u = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / denominator;
            point = Vector2.Lerp(a, b, t);

            return t is >= 0 and <= 1 && u is >= 0 and <= 1;
        }

        public static bool LinesIntersect(
            Vector2 a,
            Vector2 b,
            Vector2[] polygon,
            bool isClosedShape,
            out Vector2 point
        ) {
            point = Vector2.zero;

            for (var i = 0; i < polygon.Length - 1; i++) {
                Vector2 c = polygon[i];
                Vector2 d = polygon[i + 1];
                if (LinesIntersect(a, b, c, d, out point)) return true;
            }

            return isClosedShape && LinesIntersect(a, b, polygon[^1], polygon[0], out point);
        }

        public static Vector2 Rotate(Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static bool IsInsideCircle(Vector2 point, Vector2 center, float radius)
            => Vector2.Distance(point, center) <= radius;

        public static bool RayIntersectsCircle(
            Vector2 rayOrigin,
            Vector2 rayDirection,
            Vector2 circleCenter,
            float circleRadius,
            out Vector2 intersection1,
            out Vector2 intersection2
        ) {
            return SolveCircleIntersection(rayOrigin, rayDirection, circleCenter, circleRadius, float.PositiveInfinity, out intersection1, out intersection2);
        }

        public static bool LineIntersectsCircle(
            Vector2 lineStart,
            Vector2 lineEnd,
            Vector2 circleCenter,
            float circleRadius,
            out Vector2 intersection1,
            out Vector2 intersection2
        ) {
            Vector2 lineDirection = lineEnd - lineStart;
            return SolveCircleIntersection(lineStart, lineDirection, circleCenter, circleRadius, 1.0f, out intersection1, out intersection2);
        }

        private static bool SolveCircleIntersection(
            Vector2 origin,
            Vector2 direction,
            Vector2 circleCenter,
            float circleRadius,
            float maxT,
            out Vector2 intersection1,
            out Vector2 intersection2
        ) {
            intersection1 = Vector2.zero;
            intersection2 = Vector2.zero;

            Vector2 relativeOrigin = origin - circleCenter;
            circleRadius *= 0.5f;

            float a = Vector2.Dot(direction, direction);
            float b = 2 * Vector2.Dot(direction, relativeOrigin);
            float c = Vector2.Dot(relativeOrigin, relativeOrigin) - circleRadius * circleRadius;

            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
                return false;

            float sqrtDiscriminant = Mathf.Sqrt(discriminant);

            if (discriminant == 0) {
                float t = -b / (2 * a);

                if (t >= 0 && t <= maxT) {
                    intersection1 = origin + t * direction;
                    return true;
                }

                return false;
            }

            float t1 = (-b + sqrtDiscriminant) / (2 * a);
            float t2 = (-b - sqrtDiscriminant) / (2 * a);

            var hasIntersection = false;

            if (t1 >= 0 && t1 <= maxT) {
                intersection1 = origin + t1 * direction;
                hasIntersection = true;
            }

            if (t2 >= 0 && t2 <= maxT) {
                intersection2 = origin + t2 * direction;
                hasIntersection = true;
            }

            return hasIntersection;
        }

        public static bool CircleIntersectsPolygon(Vector2 circleCenter, float circleRadius, Vector2[][] polygons, bool isClosed = true) {
            foreach (Vector2[] polygon in polygons) {
                Vector2 wallStart = polygon[^1];
                Vector2 wallEnd = polygon[0];

                if (isClosed && LineIntersectsCircle(wallStart, wallEnd, circleCenter, circleRadius, out _, out _))
                    return true;

                for (var i = 1; i < polygon.Length; i++) {
                    wallStart = polygon[i - 1];
                    wallEnd = polygon[i];

                    if (LineIntersectsCircle(wallStart, wallEnd, circleCenter, circleRadius, out _, out _))
                        return true;
                }
            }

            return false;
        }
    }
}