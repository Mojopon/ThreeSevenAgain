using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;
using NSubstitute;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using System.Linq;

[TestFixture]
public class GeometryTest
{
    [Test]
    public void Cant_Add_Same_Point_To_the_HashSet()
    {
        var hashSet = new HashSet<Point<int>>();

        var pointOne   = new Point<int>() { X = 3, Y = 4 };
        var pointTwo   = new Point<int>() { X = 3, Y = 4 };
        var pointThree = new Point<int>() { X = 5, Y = 4 };

        Assert.IsTrue (hashSet.Add(pointOne));
        Assert.IsFalse(hashSet.Add(pointOne));
        Assert.IsFalse(hashSet.Add(pointTwo));
        Assert.IsTrue (hashSet.Add(pointThree));
    }
}
