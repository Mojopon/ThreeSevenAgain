using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using ThreeSeven.Model;
using System;

[TestFixture]
public class BlockTest
{
    [Test]
    public void Create_Block()
    {
        // to Create Block
        // you need to give it a Number

        Action<Block, int> assertion = (block, number) =>
        {
            Assert.AreEqual((ThreeSevenBlock)number, block.Type);
            Assert.AreEqual(number, block.GetNumber());
        };

        assertion(Block.Create(ThreeSevenBlock.One), 1);
        assertion(Block.Create(ThreeSevenBlock.Two), 2);
        assertion(Block.Create(ThreeSevenBlock.Three), 3);
        assertion(Block.Create(ThreeSevenBlock.Four), 4);
        assertion(Block.Create(ThreeSevenBlock.Five), 5);
        assertion(Block.Create(ThreeSevenBlock.Six), 6);
        assertion(Block.Create(ThreeSevenBlock.Seven), 7);

        // blocks have no number will returns zero
        assertion(Block.Create(ThreeSevenBlock.None), 0);
    }
}
