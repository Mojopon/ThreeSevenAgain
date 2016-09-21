using System.Collections;
using NUnit.Framework;
using ThreeSeven.Helper;
using ThreeSeven.Model;
using NSubstitute;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using System.Linq;

[TestFixture]
public class GridResolveTest
{
    [Test]
    public void Should_Resolve_Block_When_Number_Is_Lined_By_Seven()
    {
        int[,] grid;
        grid = new int[,]
        {
            {0, 0, 0 },
            {0, 3, 0 },
            {0, 4, 0 },
            {0, 4, 0 },
        };
        grid = RotateGrid(grid);

        Point<int>[] resolvedPoints;
        resolvedPoints = grid.ResolveThreeSevenGrid();

        Assert.AreEqual(2, resolvedPoints.Length);
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() {X = 1, Y = 1 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() {X = 1, Y = 2 }));

        grid = new int[,]
        {
            {0, 0, 0, 0 },
            {2, 0, 0, 0 },
            {5, 0, 2, 0 },
            {2, 7, 1, 6 },
        };
        grid = RotateGrid(grid);

        resolvedPoints = grid.ResolveThreeSevenGrid();
        Assert.AreEqual(5, resolvedPoints.Length);
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 0, Y = 1 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 0, Y = 2 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 0, Y = 3 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 2, Y = 3 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 3, Y = 3 }));
    }

    [Test]
    public void Should_Resolve_Three_Seven()
    {
        int[,] grid;
        Point<int>[] resolvedPoints;


        grid = new int[,]
        {
            {0, 0, 0 },
            {0, 7, 0 },
            {0, 7, 0 },
            {0, 7, 0 },
        };
        grid = RotateGrid(grid);

        resolvedPoints = grid.ResolveThreeSevenGrid();
        Assert.AreEqual(3, resolvedPoints.Length);
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 1, Y = 1 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 1, Y = 2 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 1, Y = 3 }));

        grid = new int[,]
        {
            {0, 0, 0, 0 },
            {0, 7, 0, 0 },
            {0, 7, 2, 0 },
            {0, 7, 1, 2 },
            {7, 7, 1, 7 },
        };
        grid = RotateGrid(grid);

        resolvedPoints = grid.ResolveThreeSevenGrid();
        Assert.AreEqual(4, resolvedPoints.Length);
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 1, Y = 1 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 1, Y = 2 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 1, Y = 3 }));
        Assert.IsTrue(resolvedPoints.Contains(new Point<int>() { X = 1, Y = 4 }));

        grid = new int[,]
        {
            {0, 0, 0, 0 },
            {0, 1, 0, 0 },
            {0, 7, 2, 0 },
            {0, 3, 6, 5 },
            {6, 7, 4, 7 },
        };
        grid = RotateGrid(grid);

        resolvedPoints = grid.ResolveThreeSevenGrid();
        Assert.AreEqual(0, resolvedPoints.Length);
    }

    private int[,] RotateGrid(int[,] origin)
    {
        var grid = new int[origin.GetLength(1), origin.GetLength(0)];

        for (int y = 0; y < origin.GetLength(1); y++)
        {
            for (int x = 0; x < origin.GetLength(0); x++)
            {
                grid[y, x] = origin[x, y];
            }
        }

        return grid;
    }
}
