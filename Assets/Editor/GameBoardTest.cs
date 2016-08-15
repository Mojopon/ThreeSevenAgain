using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;
using NSubstitute;
using System.Collections.Generic;
using System;
using UnityEngine;

[TestFixture]
public class GameBoardTest
{
    GameBoard gameBoard;
    [SetUp]
    public void Initialize()
    {
        var width = 7;
        var height = 14;
        var mask = 2;

        gameBoard = new GameBoard(new Size<int> { Width = width, Height = height });
        gameBoard.TopMask = mask;
        gameBoard.NumberOfNextTetrominos = 1;
    }

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

    [Test]
    public void Can_Set_TetrominoFactory()
    {
        var blocks = GetBlocksFromOneToSeven();
        Func<Block> blockFactory = () =>
        {
            var block = blocks[0];
            blocks.RemoveAt(0);
            return block;
        };

        var tetrominoFactory = Substitute.For<ITetrominoFactory>();
        tetrominoFactory.Create.Returns(() =>
        {
            return Tetromino.Create(Polyomino.Create(PolyominoIndex.I), blockFactory);
        });

        gameBoard.TopMask = 0;
        gameBoard.SetTetrominoFactory(tetrominoFactory);
        gameBoard.Start();

        var currentTetrominoBlocks = gameBoard.CurrentTetrominoBlocks;
        // Polyomino of I should has 4 blocks in it
        Assert.AreEqual(4, currentTetrominoBlocks.Length);

        // one to four blocks should have been set after a tetromino is created
        for(int i = 0; i < currentTetrominoBlocks.Length; i++)
        {
            Assert.AreEqual(i + 1, currentTetrominoBlocks[i].GetNumber());
        }
    }

    private List<Block> GetBlocksFromOneToSeven()
    {
        return new List<Block>()
        {
            Block.Create(ThreeSevenBlock.One),
            Block.Create(ThreeSevenBlock.Two),
            Block.Create(ThreeSevenBlock.Three),
            Block.Create(ThreeSevenBlock.Four),
            Block.Create(ThreeSevenBlock.Five),
            Block.Create(ThreeSevenBlock.Six),
            Block.Create(ThreeSevenBlock.Seven),
        };
    }
}
