using UnityEngine;
using System.Collections;

namespace ThreeSeven.Model
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public static class DirectionExtensions
    {
        public static Point<int> ToPoint(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Point<int>() { X = 0, Y = -1 };
                case Direction.Down:
                    return new Point<int>() { X = 0, Y = 1 };
                case Direction.Left:
                    return new Point<int>() { X = -1, Y = 0 };
                case Direction.Right:
                    return new Point<int>() { X = 1, Y = 0 };
            }

            return new Point<int>();
        }
    }
}