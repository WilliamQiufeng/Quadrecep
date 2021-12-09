using Godot;

namespace Quadrecep
{
    public static class GeometryHelper
    {
        public static bool WillPassRegion(Vector2 linePos1, Vector2 linePos2, Vector2 regionPos1, Vector2 regionPos2)
        {
            return IntersectWithVerticalBorders(linePos1, linePos2, regionPos1, regionPos2) ||
                   IntersectWithVerticalBorders(linePos1.Exchange(), linePos2.Exchange(), regionPos1.Exchange(),
                       regionPos2.Exchange());
        }

        private static bool IntersectWithVerticalBorders(Vector2 linePos1, Vector2 linePos2, Vector2 regionPos1,
            Vector2 regionPos2)
        {
            var m = (linePos2.y - linePos1.y) / (linePos2.x - linePos1.x);
            var b = linePos1.y - m * linePos1.x;
            var intersectionY1 = m * regionPos1.x + b;
            var intersectionY2 = m * regionPos2.x + b;
            return regionPos1.y <= intersectionY1 && intersectionY1 <= regionPos2.y ||
                   regionPos1.y <= intersectionY2 && intersectionY2 <= regionPos2.y;
        }

        private static Vector2 Exchange(this Vector2 vec) => new(vec.y, vec.x);
    }
}