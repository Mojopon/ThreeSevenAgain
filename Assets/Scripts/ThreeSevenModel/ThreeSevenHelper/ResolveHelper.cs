﻿using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using System.Collections.Generic;
using System.Linq;

namespace ThreeSeven.Helper
{
    public static class ResolveHelper
    {
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
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Up,    targetNumber) == true ? true : resolved;
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Down,  targetNumber) == true ? true : resolved;
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Left , targetNumber) == true ? true : resolved;
                resolved = ResolveNumberRecursive(grid, source, pointsToResolve, Direction.Right, targetNumber) == true ? true : resolved;
            }
            else if(targetNumber == 7)
            {
                resolved = ResolveSevenRecursive(grid, source, pointsToResolve, Direction.Up,    1) == true ? true : resolved;
                resolved = ResolveSevenRecursive(grid, source, pointsToResolve, Direction.Down,  1) == true ? true : resolved;
                resolved = ResolveSevenRecursive(grid, source, pointsToResolve, Direction.Left,  1) == true ? true : resolved;
                resolved = ResolveSevenRecursive(grid, source, pointsToResolve, Direction.Right, 1) == true ? true : resolved;
            }

            if (resolved)
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

        private static bool ResolveSevenRecursive(int[,] grid, Point<int> previous, HashSet<Point<int>> pointsToResolve, Direction direction, int count)
        {
            var current = previous.Add(direction.ToPoint());

            if (grid.IsOutOfRange(current)) return false;
            if (grid[current.X, current.Y] == 0
             || grid[current.X, current.Y] != 7) return false;

            count++;

            if (count == 3)
            {
                pointsToResolve.Add(current);
                return true;
            }

            if (ResolveSevenRecursive(grid, current, pointsToResolve, direction, count))
            {
                pointsToResolve.Add(current);
                return true;
            }

            return false;
        }

    }

}