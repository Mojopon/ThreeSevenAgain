using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using System.Collections.Generic;
using System.Linq;

namespace ThreeSeven.Helper
{
    public static class ResolveHelper
    {
        internal static Point<int> ToPoint(this Direction direction)
        {
            switch(direction)
            {
                case Direction.Up:
                    return new Point<int>() { X = 0 , Y = -1 };
                case Direction.Down:
                    return new Point<int>() { X = 0 , Y = 1 };
                case Direction.Left:
                    return new Point<int>() { X = -1, Y = 0 };
                case Direction.Right:
                    return new Point<int>() { X = 1 , Y = 0 };
            }

            return new Point<int>();
        } 

        internal enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        public static Point<int>[] ResolveThreeSevenGrid(this int[,] @this)
        {
            var pointsToResolve = new HashSet<Point<int>>();

            @this.ForEach((point, number) => StartResolveRecursive(@this, point, pointsToResolve));

            return pointsToResolve.ToArray();
        }

        private static void StartResolveRecursive(int[,] grid, Point<int> source, HashSet<Point<int>> pointsToResolve)
        {
            bool resolved = false;

            var targetNumber = grid[source.X, source.Y];

            if (targetNumber <= 0) return;

            // do this when target number is between 1 to 6
            if (targetNumber < 7)
            {
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Up, targetNumber) == true ? true : resolved;
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Down, targetNumber) == true ? true : resolved;
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Left, targetNumber) == true ? true : resolved;
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Right, targetNumber) == true ? true : resolved;
            }

            if(resolved)
            {
                pointsToResolve.Add(source);
            }
        }

        private static bool ResolveNumberRecursive(int[,] grid, Point<int> previous, HashSet<Point<int>> pointsToResolve, Direction direction, int total)
        {
            var current = previous.Add(direction.ToPoint());

            if (grid.IsOutOfRange(current)) return false;
            if (grid[current.X, current.Y] == 0 
             || grid[current.X, current.Y] >= 7) return false;

            total += grid[current.X, current.Y];

            if(total == 7)
            {
                pointsToResolve.Add(current);
                return true;
            }

            if(ResolveNumberRecursive(grid, current, pointsToResolve, direction, total))
            {
                pointsToResolve.Add(current);
                return true;
            }

            return false;
        }

    }

}