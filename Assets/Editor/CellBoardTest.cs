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
}
