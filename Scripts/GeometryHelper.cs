using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Quadrecep
{
    public static class GeometryHelper
    {
        public static bool InsideRegion(Vector2 point, Vector2 regionPos1, Vector2 regionPos2)
        {
            return regionPos1.x <= point.x && point.x <= regionPos2.x &&
                   regionPos1.y <= point.y && point.y <= regionPos2.y;
        }

        public static IEnumerable<Vector2> IntersectionWithRegion(Vector2 linePos1, Vector2 linePos2,
            Vector2 regionPos1, Vector2 regionPos2)
        {
            var res = IntersectionWithVerticalBorders(linePos1, linePos2, regionPos1, regionPos2);
            res.AddRange(IntersectionWithVerticalBorders(linePos1.Exchange(), linePos2.Exchange(),
                regionPos1.Exchange(),
                regionPos2.Exchange()).Select(x => x.Exchange()));
            return res;
        }

        private static List<Vector2> IntersectionWithVerticalBorders(Vector2 linePos1, Vector2 linePos2,
            Vector2 regionPos1,
            Vector2 regionPos2)
        {
            var res = new List<Vector2>();
            var m = (linePos2.y - linePos1.y) / (linePos2.x - linePos1.x);
            var b = linePos1.y - m * linePos1.x;
            var intersectionY1 = m * regionPos1.x + b;
            var intersectionY2 = m * regionPos2.x + b;
            if (regionPos1.y <= intersectionY1 && intersectionY1 <= regionPos2.y)
                res.Add(new Vector2(regionPos1.x, intersectionY1));

            if (regionPos1.y <= intersectionY2 && intersectionY2 <= regionPos2.y)
                res.Add(new Vector2(regionPos2.x, intersectionY2));

            return res;
        }

        private static Vector2 Exchange(this Vector2 vec) => new(vec.y, vec.x);
    }
}