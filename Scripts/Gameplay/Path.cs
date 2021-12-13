using System;
using System.Linq;
using Godot;
using Quadrecep.Map;

namespace Quadrecep.Gameplay
{
    public class Path
    {
        private static readonly float SqrtHalf = (float) Math.Sqrt(0.5f);

        public readonly float Factor;

        private Vector2 _k, _p;
        public Vector2 Direction;

        /// <summary>
        ///     Number of pixels the player goes in one second.
        ///     For directions not parallel to x or y axis, if the player travels
        ///     from (0, 0) to (x, y) in one second, the speed should be sqrt(x^2+y^2)
        /// </summary>
        public float Speed;

        public Vector2 StartPosition, EndPosition;

        public float StartTime, EndTime;
        public NoteObject TargetNote;

        public Path(float sv, float factor, float startTime, float endTime, Vector2 direction, Vector2 startPosition,
            NoteObject targetNote)
        {
            Factor = factor;
            Speed = sv * Factor;
            StartTime = startTime;
            EndTime = endTime;
            Direction = direction;
            StartPosition = startPosition;
            TargetNote = targetNote;
            CalculateConstants();
            EndPosition = this[EndTime];
        }

        public float SV => Speed / Factor;
        public bool NotMoving => Direction == Vector2.Zero;
        public bool IsInstant => StartTime == EndTime;

        public Vector2 this[float time, bool round = false] => round ? GetPositionRounded(time) : GetPosition(time);
        public float this[Vector2 position] => GetTime(position);

        /// The following explains how to deduce where the player is accurately.
        /// 
        /// Let v be the speed, (x, y) be the initial position of the player,
        /// (dx, dy) be the direction of the note (where dx and dy is in set {-1, 0, 1})
        /// t1 be the initial time and t2 be the final time.
        /// 
        /// The distance travelled in any direction in a constant time should be constant:
        /// Since dx^2 and dy^2 can only be either 0 or 1,
        /// There should be a factor C (SqrtHalf), where sqrt((Cdx)^2 + (Cdy)^2) = 1:
        /// when dx^2 = dy^2 = 1: 2C^2 = 1, C = sqrt(1/2)
        /// when dx^2 + dy^2 is less than 2, dx and dy in {-1, 0, 1}: C = 1.
        /// Optional: C can be calculated without using conditions:
        /// C = sqrt(1/(dx^2+dy^2))
        /// 
        /// From that we can calculate the final position the player is at at time t2:
        /// Pos = (x + vCdx(t2 - t1)/1000, y + vCdy(t2 - t1)/1000)
        /// When the player is moving from first note to the second at t between t2 and t1:
        /// Pos = (x + vCdx(t - t1)/1000, y + vCdy(t - t1)/1000)
        /// While v, C, dx, dy and 1000 are constants, we can define constants Kx, Ky:
        /// Kx = vCdx/1000, Ky = vCdy/1000
        /// That way the position of the player at t can be calculated as follow:
        /// Pos = (x + Kx(t - t1), y + Ky(t - t1))
        /// which can be further shrunk by defining Px = x - Kxt1, Py = y - Kyt1:
        /// Pos = (Px + Kx * t, Py + Ky * t)
        /// <summary>Calculates the player's position at a given time, given that the time is within the range</summary>
        /// <param name="time">Time (Absolute)</param>
        /// <returns>The player's position at the given time</returns>
        public Vector2 GetPosition(float time)
        {
            return _p + _k * time;
        }

        public Vector2 GetPositionRounded(float time)
        {
            return GetPosition(time).Round();
        }

        public float GetTime(Vector2 position)
        {
            var time = (position - _p) / _k;
            return float.IsNaN(time.x) ? float.IsNaN(time.y) ? 0 : time.y : time.x;
        }

        public bool WithinTime(float time)
        {
            return StartTime <= time && time <= EndTime;
        }

        public bool WithinTime(Vector2 position)
        {
            return WithinTime(this[position]);
        }

        /// <summary>
        ///     Calculates constants (K, P) used for <see cref="GetPosition" />.
        ///     See comments above.
        /// </summary>
        public void CalculateConstants()
        {
            var c = (float) Math.Sqrt(1d / (Direction.x * Direction.x + Direction.y * Direction.y));
            if (float.IsInfinity(c)) c = 0;
            _k = Direction * c * Speed / 1000;
            _p = StartPosition - _k * StartTime;
        }


        public override string ToString()
        {
            return
                $"[Path: {nameof(Speed)}: {Speed}, {nameof(StartTime)}: {StartTime}, {nameof(EndTime)}: {EndTime}, {nameof(Direction)}: {Direction}, {nameof(StartPosition)}: {StartPosition}, {nameof(EndPosition)}: {EndPosition}, {nameof(TargetNote)}: {TargetNote}]";
        }

        public static Path CutVisiblePath(Path path, Vector2 regionPos1, Vector2 regionPos2)
        {
            var intersections = GeometryHelper
                .IntersectionWithRegion(path.StartPosition, path.EndPosition, regionPos1, regionPos2)
                .Select(x => new {pos = x, time = path[x]}).ToList();
            if (path.NotMoving && GeometryHelper.InsideRegion(path.StartPosition, regionPos1, regionPos2)) return path;
            if (intersections.Count <= 1) return null;
            var startTime = intersections.Min(x => x.time);
            var endTime = intersections.Max(x => x.time);
            if (!path.WithinTime(startTime)) startTime = path.StartTime;
            if (!path.WithinTime(endTime)) endTime = path.EndTime;
            var cutPath = new Path(path.SV, path.Factor, startTime, endTime, path.Direction,
                path[startTime], path.TargetNote);
            return cutPath.IsInstant ? null : cutPath;
        }
    }
}