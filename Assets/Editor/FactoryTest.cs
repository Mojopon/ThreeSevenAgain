using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;

[TestFixture]
public class FactoryTest
{
    [SetUp]
    public void Initialize() { }

    [Test]
    public void Should_Return_from_One_to_Seven_By_Default()
    {
        var factory = new TetrominoFactory();
        int count = 0;

        Tetromino tetromino;
        tetromino = factory.Create();
        tetromino.Foreach((point, block) =>
        {
            Assert.AreEqual(block.GetNumber(), ++count);

            if (count >= 7) count = 0;
        });

        tetromino = factory.Create();
        tetromino.Foreach((point, block) =>
        {
            Assert.AreEqual(block.GetNumber(), ++count);

            if (count >= 7) count = 0;
        });
    }
}
