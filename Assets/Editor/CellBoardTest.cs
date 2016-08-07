using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;

[TestFixture]
public class CellBoardTest
{
    [SetUp]
    public void Initialize() { }

    [Test]
    public void Setup_CellBoard_By_Size()
    {
        var width = 7;
        var height = 14;

        var cellboard = new CellBoard(new Size<int> { Width = width, Height = height });
        Assert.AreEqual(cellboard.Size.Width, width);
        Assert.AreEqual(cellboard.Size.Height, height);

        var cellsClone = cellboard.CellsClone;
        Assert.AreEqual(cellsClone.GetLength(0), width);
        Assert.AreEqual(cellsClone.GetLength(1), height);
    }

    // ActualCells should return CellBoard which has top of the board to be masked.
    // Cells is the original board.
    [Test]
    public void It_Should_Return_ActualCells()
    {
        var width = 7;
        var height = 14;

        var maskedCellboard = new MaskedCellBoard(new Size<int> { Width = width, Height = height });

        var mask = 4;
        maskedCellboard.TopMask = mask;

        Assert.AreEqual(maskedCellboard.Size.Width , width);
        Assert.AreEqual(maskedCellboard.Size.Height, height);

        Assert.AreEqual(maskedCellboard.ActualSize.Width , width);
        Assert.AreEqual(maskedCellboard.ActualSize.Height, height - mask);

        var cellsClone = maskedCellboard.CellsClone;
        Assert.AreEqual(cellsClone.GetLength(0), width);
        Assert.AreEqual(cellsClone.GetLength(1), height);

        var actualCells = maskedCellboard.ActualCells;
        Assert.AreEqual(actualCells.GetLength(0), width);
        Assert.AreEqual(actualCells.GetLength(1), height - mask);

    }
}
