using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using ThreeSeven.Model;
using System;
using System.Linq;

[TestFixture]
public class TetrominoTest
{
    Tetromino tetromino;

    List<Point<int>[]> patterns = PolyominoTestFixture.patterns;
    ThreeSevenBlock[] reservedBlocks = new ThreeSevenBlock[]
                {
                    ThreeSevenBlock.One,
                    ThreeSevenBlock.Three,
                    ThreeSevenBlock.Five,
                    ThreeSevenBlock.Seven,
                };

    [SetUp]
    public void Initialize()
    {
        List<ThreeSevenBlock> nextBlocks = new List<ThreeSevenBlock>();

        Func<Block> produceBlock = () =>
        {
            if (nextBlocks.Count == 0)
            {
                nextBlocks = reservedBlocks.ToList();
            }

            var next = nextBlocks[0];
            nextBlocks.RemoveAt(0);
            return Block.Create(next);
        };

        tetromino = Tetromino.Create(Polyomino.Create(patterns), produceBlock);
    }

    [Test]
    public void Create_Tetromino()
    {
        var polyomino = Polyomino.Create(patterns);

        List<ThreeSevenBlock> nextBlocks = new List<ThreeSevenBlock>();
        int produceBlockCalled = 0;
        Func<Block> produceBlock = () =>
        {
            if(nextBlocks.Count == 0)
            {
                nextBlocks = reservedBlocks.ToList();
            }

            var next = nextBlocks[0];
            nextBlocks.RemoveAt(0);

            produceBlockCalled++;
            return Block.Create(next);
        };

        // to Create Tetromino
        // you need to give it a Polyomino(shape) and Func<Block> to produce blocks components of it
        var tetromino = Tetromino.Create(polyomino, produceBlock);

        Assert.AreEqual(produceBlockCalled, polyomino.Length);
        Assert.AreEqual(polyomino.Length, tetromino.Length);

        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(reservedBlocks[count], block.Type);
        });
    }

    [Test]
    public void Should_Return_Blocks_Positions_Related_To_Position()
    {
        tetromino.Position = new Point<int>() { X = 3, Y = 2 };

        var pattern = patterns[0];
        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(point, pattern[count].Add(tetromino.Position));
        });
    }

    [Test]
    public void Can_Change_Shape_To_Next_Pattern()
    {
        Point<int>[] pattern;

        pattern = patterns[0];
        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(point, pattern[count].Add(tetromino.Position));
        });

        pattern = patterns[1];
        tetromino.Turn(true);
        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(point, pattern[count].Add(tetromino.Position));
        });

        pattern = patterns[2];
        tetromino.Turn(true);
        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(point, pattern[count].Add(tetromino.Position));
        });

        pattern = patterns[3];
        tetromino.Turn(true);
        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(point, pattern[count].Add(tetromino.Position));
        });

        // rotate back to original pattern from the beginning
        pattern = patterns[0];
        tetromino.Turn(true);
        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(point, pattern[count].Add(tetromino.Position));
        });
    }
}
