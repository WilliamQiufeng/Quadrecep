using Godot;

namespace Quadrecep.Scripts.Map
{
    public class PathObject
    {
        private int _rawDirection;
        private int[] _direction = new int[4];
        public Vector2 NetDirection { get; private set; }

        public int[] Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                CalculateNetDirection();
                CalculateRawDirection();
            }
        }

        public int RawDirection
        {
            get => _rawDirection;
            set
            {
                _rawDirection = value;
                CalculateDirection();
                CalculateNetDirection();
            }
        }

        public void CalculateDirection()
        {
            for (var i = 3; i >= 0; i--)
            {
                // int val = (RawDirection & (3 << (i * 2))) >> (2 * i);
                // GD.Print(val);
                // // Direction[3 - i] = -(RawDirection & (2 * i + 1) * 2 - 1)  * RawDirection & (1 << (2 * i));
                // Direction[3 - i] = -(val >> 1) * (val & 1);
                _direction[3 - i] = (_rawDirection >> i) & 1;
            }
        }

        public void CalculateNetDirection()
        {
            NetDirection = new Vector2(_direction[3] - _direction[0], _direction[1] - _direction[2]);
        }

        public void CalculateRawDirection()
        {
            _rawDirection = 0;
            for (var i = 0; i < 4; i++)
            {
                // int sign = Math.Sign(Direction[i]) < 0 ? 1 : 0;
                // RawDirection <<= 2;
                // RawDirection |= (sign << 1) | (Direction[i] & 1);
                _rawDirection <<= 1;
                _rawDirection |= _direction[i] & 1;
            }
        }

        public PathObject(int rawDirection = default)
        {
            RawDirection = rawDirection;
        }

        public PathObject(int[] direction)
        {
            Direction = direction;
        }
    }
}
