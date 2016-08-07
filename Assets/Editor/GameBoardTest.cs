using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;

[TestFixture]
public class GameBoardTest
{
    [Test]
    public void Setup_GameBoard_By_Size()
    {
        var width = 7;
        var height = 14;
        var mask = 2;

        var gameBoard = new GameBoard(new Size<int> { Width = width, Height = height });
        Assert.AreEqual(gameBoard.Size.Width, width);
        Assert.AreEqual(gameBoard.Size.Height, height);

        var cellsClone = gameBoard.CellsClone;
        Assert.AreEqual(cellsClone.GetLength(0), width);
        Assert.AreEqual(cellsClone.GetLength(1), height);

        var actualCells = gameBoard.ActualCells;
        Assert.AreEqual(actualCells.GetLength(0), width);
        Assert.AreEqual(actualCells.GetLength(1), height - mask);
    }
}
