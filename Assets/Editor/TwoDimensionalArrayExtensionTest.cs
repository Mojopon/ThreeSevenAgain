using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;
using NSubstitute;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;

[TestFixture]
public class TwoDimensionalArrayExtensionTest
{
    [Test]
    public void Should_be_True_When_Out_of_Range()
    {
        int width = 4;
        int height = 5;

        int[,] array = new int[width, height];

        for (int y = -5; y < height + 5; y++)
        {
            for (int x = -5; x < width + 5; x++)
            {
                if (0 > x || 0 > y || x >= width || y >= height)
                {
                    Assert.IsTrue(array.IsOutOfRange(Point<int>.At(x, y)));
                }
                else
                {
                    Assert.IsFalse(array.IsOutOfRange(Point<int>.At(x, y)));
                }
            }
        }
    }

    [Test]
    public void Should_Swap_Two_Dimensional_Array_Element()
    {
        int width = 4;
        int height = 5;

        int[,] array = new int[width, height];

        array[0, 0] = 5;

        array.Swap(Point<int>.At(0, 0), Point<int>.At(0, 1));
        Assert.AreEqual(0, array.Get(Point<int>.At(0, 0)));
        Assert.AreEqual(5, array.Get(Point<int>.At(0, 1)));
    }

}
