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

    Func<Block> blockFactory;
    ITetrominoFactory tetrominoFactory;

    [SetUp]
    public void Initialize()
    {
        var width = 7;
        var height = 14;

        gameBoard = new GameBoard(new Size<int> { Width = width, Height = height });
        gameBoard.NumberOfNextTetrominos = 1;
    }

    [Test]
    public void Setup_GameBoard_By_Size()
    {
        var width = 7;
        var height = 14;

        var gameBoard = new GameBoard(new Size<int> { Width = width, Height = height });
        Assert.AreEqual(gameBoard.Size.Width, width);
        Assert.AreEqual(gameBoard.Size.Height, height);

        var cellsClone = gameBoard.CellsClone;
        Assert.AreEqual(cellsClone.GetLength(0), width);
        Assert.AreEqual(cellsClone.GetLength(1), height);
    }

    [Test]
    public void Can_Set_TetrominoFactory()
    {
        var blocks       = GetBlocksFromOneToSeven();
        var patterns     = PolyominoTestFixture.patterns;
        blockFactory     = PrepareBlockFactory();
        tetrominoFactory = PrepareTetrominoFactory(patterns, blockFactory);

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

        var pattern = patterns[0];

        // tetromino should be added to the center of the gameboard.
        // it also counts tetromino size.
        var xPointToPlaceTetromino = gameBoard.Center.X - gameBoard.CurrentTetromino.Size.Width / 2;
        Assert.AreEqual(xPointToPlaceTetromino, gameBoard.CurrentTetromino.Position.X);

        // check if the tetromino blocks are placed to the positions it should be.
        for (int i = 0; i < currentTetrominoBlocks.Length; i++)
        {
            Assert.AreEqual(xPointToPlaceTetromino + pattern[i].X, gameBoard.CurrentTetrominoPositions[i].X);
            Assert.AreEqual(pattern[i].Y, gameBoard.CurrentTetrominoPositions[i].Y);
        }

        var cellsWithoutMasked = gameBoard.CellsClone;
    }

    [Test]
    public void Can_Move_Tetromino()
    {

    }

    private Func<Block> PrepareBlockFactory()
    {
        var blocks = GetBlocksFromOneToSeven();

        return () =>
        {
            var block = blocks[0];
            blocks.RemoveAt(0);
            return block;
        };
    }

    private ITetrominoFactory PrepareTetrominoFactory(List<Point<int>[]> patterns, Func<Block> blockFactory)
    {
        var tetrominoFactoryMock = Substitute.For<ITetrominoFactory>();

        tetrominoFactoryMock.Create.Returns(() =>
        {
            return Tetromino.Create(Polyomino.Create(patterns), blockFactory);
        });

        return tetrominoFactoryMock;
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
