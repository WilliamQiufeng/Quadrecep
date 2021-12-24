using System;
using System.Linq;
using Godot;
using Quadrecep.GameMode.Navigate.Map;
using YamlDotNet.Serialization;

namespace Quadrecep.Gameplay
{
    public class Path
    {
        public readonly float Factor;

        [YamlIgnore] private Vector2 _k, _p;
        public Vector2 Direction;
        [YamlIgnore] public Vector2 EndPosition;
        [YamlIgnore] public bool IsInstant;
        [YamlIgnore] public bool NotMoving;

        /// <summary>
        ///     Number of pixels the player goes in one second.
        ///     For directions not parallel to x or y axis, if the player travels
        ///     from (0, 0) to (x, y) in one second, the speed should be sqrt(x^2+y^2)
        /// </summary>
        [YamlIgnore] public float Speed;

        public Vector2 StartPosition;

        public float StartTime, EndTime;

        public float SV;
        [YamlIgnore] public NoteObject TargetNote;

        public Path(float sv, float factor, float startTime, float endTime, Vector2 direction, Vector2 startPosition,
            NoteObject targetNote)
        {
            Factor = factor;
            SV = sv;
            StartTime = startTime;
            EndTime = endTime;
            Direction = direction;
            StartPosition = startPosition;
            TargetNote = targetNote;
            CalculateConstants();
        }

        public Path()
        {
        }

        public Vector2 this[float time, bool round = false] => round ? GetPositionRounded(time) : GetPosition(time);
        public float this[Vector2 position] => GetTime(position);


        /// <summary>
        ///     <para>
        ///         Calculates the player's position at a given time, given that the time is within the range
        ///         The following explains how to deduce where the player is accurately.<br />
        ///     </para>
        ///     Let <c>v</c> be the speed, <c>(x, y)</c> be the initial position of the player,<br />
        ///     <c>(dx, dy)</c> be the direction of the note<br />
        ///     <c>t1</c> be the initial time and <c>t2</c> be the final time.<br />
        ///     The distance travelled in any direction in a constant time should be constant:<br />
        ///     There should be a factor <c>C</c>, where <c>sqrt((Cdx)^2 + (Cdy)^2) = 1</c>.<br />
        ///     C can be calculated by the following:<br />
        ///     <code>C = sqrt(1/(dx^2+dy^2))</code>
        ///     From that we can calculate the final position the player is at at time t2:<br />
        ///     <code>Pos = (x + vCdx(t2 - t1)/1000, y + vCdy(t2 - t1)/1000)</code>
        ///     When the player is moving from first note to the second at t between t2 and t1:<br />
        ///     <code>Pos = (x + vCdx(t - t1)/1000, y + vCdy(t - t1)/1000)</code>
        ///     While v, C, dx, dy and 1000 are constants, we can define constants Kx, Ky:<br />
        ///     <code>Kx = vCdx/1000, Ky = vCdy/1000</code>
        ///     That way the position of the player at t can be calculated as follow:<br />
        ///     <code>Pos = (x + Kx(t - t1), y + Ky(t - t1))</code>
        ///     which can be further shrunk by defining Px = x - Kxt1, Py = y - Kyt1:<br />
        ///     <code>Pos = (Px + Kx * t, Py + Ky * t)</code>
        ///     It is possible to use vector directly in calculation, so the final expression looks like this:
        ///     <code>Pos = P + K * t</code>
        /// </summary>
        /// <param name="time">Time (Absolute)</param>
        /// <returns>The player's position at the given time</returns>
        public Vector2 GetPosition(float time)
        {
            return _p + _k * time;
        }

        /// <summary>
        ///     Rounds output from <see cref="GetPosition" />
        /// </summary>
        /// <param name="time">abs time</param>
        /// <returns>Rounded position</returns>
        /// <seealso cref="GetPosition" />
        public Vector2 GetPositionRounded(float time)
        {
            return GetPosition(time).Round();
        }

        /// <summary>
        ///     Gets the time given position. This is the reverse process of <see cref="GetPosition" />.
        /// </summary>
        /// <param name="position">Position in the path</param>
        /// <returns>The time when the path reaches the position</returns>
        public float GetTime(Vector2 position)
        {
            var time = (position - _p) / _k;
            return float.IsNaN(time.x) ? float.IsNaN(time.y) ? 0 : time.y : time.x;
        }

        /// <summary>
        ///     Checks if the time given falls in the time interval of the current path
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns>If the time is in range of current path time</returns>
        public bool WithinTime(float time)
        {
            return StartTime <= time && time <= EndTime;
        }

        /// <summary>
        ///     Checks if there's no overlap in the time interval of the current path and the given time interval.
        /// </summary>
        /// <param name="startTime">The start time to check</param>
        /// <param name="endTime">The end time to check</param>
        /// <returns>If the two time intervals <em>don't</em> overlap</returns>
        public bool BothOutOfTime(float startTime, float endTime)
        {
            return startTime < StartTime && endTime < StartTime || startTime > EndTime && endTime > EndTime;
        }

        /// <summary>
        ///     Checks if the position falls in the time interval of the current path
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>If the time is in range of current path time</returns>
        /// <seealso cref="WithinTime(float)" />
        public bool WithinTime(Vector2 position)
        {
            return WithinTime(this[position]);
        }

        /// <summary>
        ///     Reverse the path so its start and end position swaps
        /// </summary>
        public void Reverse()
        {
            (StartPosition, EndPosition) = (EndPosition, StartPosition);
            Direction = -Direction;
            CalculateConstants();
        }

        /// <summary>
        ///     Returns a clone of this path but with itself reversed
        /// </summary>
        /// <returns>The reversed path</returns>
        public Path Reversed()
        {
            var copy = (Path) MemberwiseClone();
            copy.Reverse();
            return copy;
        }

        /// <summary>
        ///     Offset <see cref="StartPosition" /> and <see cref="EndPosition" /> by <paramref name="offset" />.
        /// </summary>
        /// <param name="offset">The offset to offset(what)</param>
        public void Offset(Vector2 offset)
        {
            StartPosition += offset;
            EndPosition += offset;
            CalculateConstants();
        }

        /// <summary>
        ///     Returns a clone of this path with itself offset
        /// </summary>
        /// <param name="offset">The offset to give to the new path</param>
        /// <returns>The path offset</returns>
        public Path WithOffset(Vector2 offset)
        {
            var copy = (Path) MemberwiseClone();
            copy.Offset(offset);
            return copy;
        }

        /// <summary>
        ///     Operator for reversing the path
        /// </summary>
        /// <param name="path">Path to reverse</param>
        /// <returns>Reversed path</returns>
        /// <seealso cref="Reversed" />
        public static Path operator ~(Path path)
        {
            return path.Reversed();
        }

        /// <summary>
        ///     Operator for adding offset
        /// </summary>
        /// <param name="path">Path to offset</param>
        /// <param name="offset">Offset to add</param>
        /// <returns>Path with offset</returns>
        /// <seealso cref="WithOffset" />
        public static Path operator +(Path path, Vector2 offset)
        {
            return path.WithOffset(offset);
        }


        /// <summary>
        ///     Calculates:<br />
        ///     <see cref="Speed" /> from <see cref="SV" /> and <see cref="Factor" />,<br />
        ///     c for vector coefficient,<br />
        ///     <see cref="_k" />, <see cref="_p" /> used for <see cref="GetPosition" />,<br />
        ///     <see cref="NotMoving" />, whether the path won't move the whole time,<br />
        ///     <see cref="IsInstant" />, whether the path starts and ends at the same time,<br />
        ///     <see cref="EndPosition" />, position when path ends.<br />
        ///     See comments above.
        /// </summary>
        public void CalculateConstants()
        {
            Speed = SV * Factor;
            var c = (float) Math.Sqrt(1d / (Direction.x * Direction.x + Direction.y * Direction.y));
            // The path doesn't move
            if (float.IsInfinity(c)) c = 0;
            _k = Direction * c * Speed / 1000;
            _p = StartPosition - _k * StartTime;
            NotMoving = Direction == Vector2.Zero || StartPosition == EndPosition;
            IsInstant = StartTime == EndTime;
            // SV = Speed / Factor;
            EndPosition = this[EndTime];
        }


        public override string ToString()
        {
            return
                $"[Path: {nameof(Speed)}: {Speed}, {nameof(StartTime)}: {StartTime}, {nameof(EndTime)}: {EndTime}, {nameof(Direction)}: {Direction}, {nameof(StartPosition)}: {StartPosition}, {nameof(EndPosition)}: {EndPosition}, {nameof(TargetNote)}: {TargetNote}]";
        }

        /// <summary>
        ///     Cuts the <paramref name="path" /> so they start and end when they are inside the specified region.
        /// </summary>
        /// <param name="path">Path to cut</param>
        /// <param name="regionPos1">Region top left</param>
        /// <param name="regionPos2">Region bottom right</param>
        /// <param name="removeInstant">Whether to return null when the cut path shows up just in an instant</param>
        /// <returns>Cut path, null if it does not pass through the region</returns>
        public static Path CutVisiblePath(Path path, Vector2 regionPos1, Vector2 regionPos2, bool removeInstant = true)
        {
            // Finds the intersections of the path to the region.
            // Retrieve the time and position of the intersections
            var intersections = GeometryHelper
                .IntersectionWithRegion(path.StartPosition, path.EndPosition, regionPos1, regionPos2)
                .Select(x => new {pos = x, time = path[x]}).ToList();
            // If the path is not moving but is inside a region, return the path directly
            if (path.NotMoving && GeometryHelper.InsideRegion(path.StartPosition, regionPos1, regionPos2))
                return path.IsInstant && removeInstant ? null : path;
            // If the path has less than or equal to one intersection with the region, return null since it won't show anyway
            if (intersections.Count <= 1) return null;
            // Find the start and end time
            var startTime = intersections.Min(x => x.time);
            var endTime = intersections.Max(x => x.time);
            // If they don't overlap the path's time interval then return null
            if (path.BothOutOfTime(startTime, endTime)) return null;
            // Constrain the times within the path's original interval
            if (!path.WithinTime(startTime)) startTime = path.StartTime;
            if (!path.WithinTime(endTime)) endTime = path.EndTime;
            // Construct cut path
            var cutPath = new Path(path.SV, path.Factor, startTime, endTime, path.Direction,
                path[startTime], path.TargetNote);
            // Return cut path
            return cutPath.IsInstant && removeInstant ? null : cutPath;
        }
    }
}