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
    }
    
    [Test]
    public void Should_Convert_To_Number_Grid()
    {
        Cells[3, 9].Set(1);
        Cells[3, 8].Set(2);
        Cells[2, 9].Set(6);

        var numberGrid = Cells.CellsToNumberGrid();

        for (int y = 0; y < numberGrid.GetLength(1); y++)
        {
            for (int x = 0; x < numberGrid.GetLength(0); x++)
            {
                if (x == 3 && y == 9 ||
                    x == 3 && y == 8 ||
                    x == 2 && y == 9) continue;

                Assert.AreEqual(0, numberGrid[x, y]);
            }
        }

        Assert.AreEqual(1, numberGrid[3, 9]);
        Assert.AreEqual(2, numberGrid[3, 8]);
        Assert.AreEqual(6, numberGrid[2, 9]);
    }

    [Test]
    public void Should_Convert_To_Bool_Grid()
    {
        Cells[3, 9].Set(1);
        Cells[3, 8].Set(2);
        Cells[2, 9].Set(6);

        var boolGrid = Cells.CellsToBoolGrid();

        for (int y = 0; y < boolGrid.GetLength(1); y++)
        {
            for (int x = 0; x < boolGrid.GetLength(0); x++)
            {
                if (x == 3 && y == 9 ||
                    x == 3 && y == 8 ||
                    x == 2 && y == 9) continue;

                // it returns true when the cell is empty
                Assert.True(boolGrid[x, y]);
            }
        }

        Assert.IsFalse(boolGrid[3, 9]);
        Assert.IsFalse(boolGrid[3, 8]);
        Assert.IsFalse(boolGrid[2, 9]);
    }
}
