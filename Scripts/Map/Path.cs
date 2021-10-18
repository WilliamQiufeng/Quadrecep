using System;
using Godot;

namespace Quadrecep.Map
{
    public class Path
    {
        private static readonly float SqrtHalf = (float) Math.Sqrt(0.5f);

        private Vector2 _k, _p;
        public DirectionObject Direction;

        /// <summary>
        ///     Number of pixels the player goes in one second.
        ///     For directions not parallel to x or y axis, if the player travels
        ///     from (0, 0) to (x, y) in one second, the speed should be sqrt(x^2+y^2)
        public float Speed;

        public Vector2 StartPosition, EndPosition;

        public float StartTime, EndTime;
        public NoteObject TargetNote;

        public Path(float factor, float startTime, float endTime, DirectionObject direction, Vector2 startPosition,
            NoteObject targetNote, float speed = 150)
        {
            Speed = speed * factor;
            StartTime = startTime;
            EndTime = endTime;
            Direction = direction;
            StartPosition = startPosition;
            TargetNote = targetNote;
            CalculateConstants();
            EndPosition = GetPosition(EndTime);
        }

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
            return (_p + _k * time).Round();
        }

        /// <summary>
        ///     Calculates constants (K, P) used for <see cref="GetPosition" />.
        ///     See comments above.
        /// </summary>
        public void CalculateConstants()
        {
            var c = Direction.NetDirection == new Vector2(1f, 1f) ? SqrtHalf : 1;
            _k = Direction.NetDirection * c * Speed / 1000;
            _p = StartPosition - _k * StartTime;
        }

        public override string ToString()
        {
            return
                $"[Path: {nameof(Speed)}: {Speed}, {nameof(StartTime)}: {StartTime}, {nameof(EndTime)}: {EndTime}, {nameof(Direction)}: {Direction}, {nameof(StartPosition)}: {StartPosition}, {nameof(EndPosition)}: {EndPosition}, {nameof(TargetNote)}: {TargetNote}]";
        }
    }
}