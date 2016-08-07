using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;

[TestFixture]
public class CellBoardTest
{
    [SetUp]
    public void Initialize() { }

    [Test]
    public void Setup_CellBoard_Size()
    {
        var width = 7;
        var height = 14;

        var cellboard = new CellBoard(new Size<int> { Width = width, Height = height });
        Assert.IsTrue(cellboard.Size.Width == width && cellboard.Size.Height == height);
    }
}
