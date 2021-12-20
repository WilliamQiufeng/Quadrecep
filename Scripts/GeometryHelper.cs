using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Quadrecep
{
    public static class GeometryHelper
    {
        /// <summary>
        /// Checks if a point is inside a specified region
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <param name="regionPos1">Region top left</param>
        /// <param name="regionPos2">Region bottom right</param>
        /// <returns></returns>
        public static bool InsideRegion(Vector2 point, Vector2 regionPos1, Vector2 regionPos2)
        {
            return regionPos1.x <= point.x && point.x <= regionPos2.x &&
                   regionPos1.y <= point.y && point.y <= regionPos2.y;
        }

        /// <summary>
        /// Finds intersection of a line with the borders of a region<br/>
        /// This works by calling <see cref="IntersectionWithVerticalBorders"/> once and once more with axis flipped.
        /// </summary>
        /// <param name="linePos1">Line start</param>
        /// <param name="linePos2">Line end</param>
        /// <param name="regionPos1">Region top left</param>
        /// <param name="regionPos2">Region bottom right</param>
        /// <returns></returns>
        public static IEnumerable<Vector2> IntersectionWithRegion(Vector2 linePos1, Vector2 linePos2,
            Vector2 regionPos1, Vector2 regionPos2)
        {
            var res = IntersectionWithVerticalBorders(linePos1, linePos2, regionPos1, regionPos2);
            res.AddRange(IntersectionWithVerticalBorders(linePos1.Exchange(), linePos2.Exchange(),
                regionPos1.Exchange(),
                regionPos2.Exchange()).Select(x => x.Exchange()));
            return res;
        }

        /// <summary>
        /// Finds intersection of a line with the vertical border of a region.
        /// </summary>
        /// <param name="linePos1">Line start</param>
        /// <param name="linePos2">Line end</param>
        /// <param name="regionPos1">Region top left</param>
        /// <param name="regionPos2">Region bottom right</param>
        /// <returns></returns>
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

        /// <summary>
        /// Exchange <see cref="Vector2.x"/> and <see cref="Vector2.y"/>
        /// </summary>
        /// <param name="vec">Source <see cref="Vector2"/> to process</param>
        /// <returns>result <see cref="Vector2"/> flipped</returns>
        private static Vector2 Exchange(this Vector2 vec)
        {
            return new Vector2(vec.y, vec.x);
        }
    }
}