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
    List<Point<int>[]> patterns = PolyominoTestFixture.patterns;

    [Test]
    public void Create_Tetromino()
    {
        var polyomino = Polyomino.Create(patterns);

        ThreeSevenBlock[] reserve = new ThreeSevenBlock[]
                {
                    ThreeSevenBlock.One,
                    ThreeSevenBlock.Three,
                    ThreeSevenBlock.Five,
                    ThreeSevenBlock.Seven,
                };

        List<ThreeSevenBlock> nextBlocks = new List<ThreeSevenBlock>();
        int produceBlockCalled = 0;
        Func<Block> produceBlock = () =>
        {
            if(nextBlocks.Count == 0)
            {
                nextBlocks = reserve.ToList();
            }

            var next = nextBlocks[0];
            nextBlocks.RemoveAt(0);

            produceBlockCalled++;
            return Block.Create(next);
        };

        // to Create Tetromino
        // you need to give it a Polyomino and Func<Block>
        var tetromino = Tetromino.Create(polyomino, produceBlock);

        Assert.AreEqual(produceBlockCalled, polyomino.Length);
        Assert.AreEqual(polyomino.Length, tetromino.Length);

        tetromino.Foreach((count, point, block) =>
        {
            Assert.AreEqual(reserve[count], block.Type);
        });
    }
}
