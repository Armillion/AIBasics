using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility {
    public static class Geometry {
        public static bool PointInPolygon(Vector2 point, ICollection<Vector2> polygon) {
            int numVertices = polygon.Count();
            double x = point.x, y = point.y;
            var inside = false;

            Vector2 p1 = polygon.ElementAt(0);
            // Loop through each edge in the polygon
            for (var i = 1; i <= numVertices; i++) {
                Vector2 p2 = polygon.ElementAt(i % numVertices);

                if (y > Mathf.Min(p1.y, p2.y)) {
                    if (y <= Mathf.Max(p1.y, p2.y)) {
                        if (x <= Mathf.Max(p1.x, p2.x)) {
                            double xIntersection = (y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;

                            if (Mathf.Approximately(p1.x, p2.x) || x <= xIntersection) {
                                inside = !inside;
                            }
                        }
                    }
                }

                p1 = p2;
            }

            return inside;
        }
    }
}