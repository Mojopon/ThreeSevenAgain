using UnityEngine;
using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;

[TestFixture]
public class CellsExtensionTest
{
    Cell[,] Cells;
    
    [SetUp]
    public void Initialize_Cells()
    {
        var size = new Size<int>() { Width = 5, Height = 10 };
        Cells = new Cell[size.Width, size.Height];

        Cells.ForEach((point, cell) =>
        {
            Cells[point.X, point.Y].Set(0);
        });

        Cells[3, 9].Set(1);
        Cells[3, 8].Set(2);
        Cells[2, 9].Set(6);


    }
}
