using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using ThreeSeven.Model;
using System;

[TestFixture]
public class PolyominoTest
{
    List<Point<int>[]> patterns = new List<Point<int>[]>() {
                                   new Point<int>[] {
                                   new Point<int>() {X = 2, Y = 0 },
                                   new Point<int>() {X = 2, Y = 1 },
                                   new Point<int>() {X = 2, Y = 2 },
                                   new Point<int>() {X = 2, Y = 3 },
                                   },
                                   new Point<int>[] {
                                   new Point<int>() {X = 3, Y = 1 },
                                   new Point<int>() {X = 2, Y = 1 },
                                   new Point<int>() {X = 1, Y = 1 },
                                   new Point<int>() {X = 0, Y = 1 },
                                   },
                                   new Point<int>[] {
                                   new Point<int>() {X = 1, Y = 3 },
                                   new Point<int>() {X = 1, Y = 2 },
                                   new Point<int>() {X = 1, Y = 1 },
                                   new Point<int>() {X = 1, Y = 0 },
                                   },
                                   new Point<int>[] {
                                   new Point<int>() {X = 0, Y = 1 },
                                   new Point<int>() {X = 1, Y = 1 },
                                   new Point<int>() {X = 2, Y = 1 },
                                   new Point<int>() {X = 3, Y = 1 },
                                   },
                };

    [Test]
    public void Create_Polyomino_by_List_of_Array_of_Points()
    {
        var polyomino = Polyomino.Create(patterns);
        Assert.AreEqual(typeof(Polyomino), polyomino.GetType());

        // number of points will be the length of the polyomino
        Assert.AreEqual(patterns[0].Length, polyomino.Length);

        // it calculates polyomino size from the occupieng space.
        // for example, if the minimum point is <0, 0> and the maximum point is <3, 3>,
        // the number of size will be <4, 4>
        Assert.AreEqual(4, polyomino.Size.Width);
        Assert.AreEqual(4, polyomino.Size.Height);
    }

    [Test]
    public void Should_Set_Positions_by_The_Patterns()
    {
        var polyomino = Polyomino.Create(patterns);
        var patternOne = patterns[0];

        ComparePolyominoPositionsToThePattern(polyomino, patternOne, (lhs, rhs) => 
        {
            Assert.AreEqual(lhs, rhs);
        });
    }

    [Test]
    public void Refer_To_Next_Pattern_As_It_Turns()
    {
        var polyomino = Polyomino.Create(patterns);

        for(int i = 0; i < polyomino.Length; i++)
        {
            ComparePolyominoPositionsToThePattern(polyomino, patterns[i], (lhs, rhs) =>
            {
                Assert.AreEqual(lhs, rhs);
            });
            polyomino.Turn();
        }

        ComparePolyominoPositionsToThePattern(polyomino, patterns[0], (lhs, rhs) =>
        {
            Assert.AreEqual(lhs, rhs);
        });
    }

    private bool ComparePolyominoPositionsToThePattern(Polyomino polyomino, IEnumerable<Point<int>> points, Action<Point<int>, Point<int>> assertion)
    {
        var count = 0;
        bool isEqual = true;

        foreach(var point in points)
        {
            if (count >= polyomino.Length)
            {
                isEqual = false;
                break;
            }

            assertion(polyomino.Points[count], point);
            isEqual = polyomino.Points[count].Equals(point) ? isEqual : false;
            count++;
        }

        return isEqual;
    }
}
