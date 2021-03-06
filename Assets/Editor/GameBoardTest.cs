﻿using System.Collections;
using NUnit.Framework;
using ThreeSeven.Model;
using NSubstitute;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using System.Linq;

[TestFixture]
public class GameBoardTest
{
    GameBoard gameBoard;

    List<Point<int>[]> patterns;
    Func<Block> blockFactory;
    ITetrominoFactory tetrominoFactory;

    CompositeDisposable subscriptions;

    // You can see the board status by this
    // Debug.Log(gameBoard);

    [SetUp]
    public void Initialize()
    {
        var width = 7;
        var height = 14;

        gameBoard = new GameBoard(new Size<int> { Width = width, Height = height });
        gameBoard.NumberOfNextTetrominos = 1;

        patterns         = PolyominoTestFixture.patterns;
        tetrominoFactory = new TetrominoFactory(() => Polyomino.Create(patterns));

        gameBoard.SetTetrominoFactory(tetrominoFactory);

        subscriptions = new CompositeDisposable();
    }

    [TearDown]
    public void DisposeSubscriptions()
    {
        subscriptions.Dispose();
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
    public void It_Will_Notify_GameBoardEvents_When_Updated()
    {
        GameBoardEvents notifiedEvent = null;
        gameBoard.GameBoardEventsObservable
                 .Subscribe(x => notifiedEvent = x)
                 .AddTo(subscriptions);
        Assert.IsNull(notifiedEvent);

        // starting GameBoard will also notify GameBoardEvents to its subscribers
        gameBoard.StartGame();
        Assert.IsNotNull(notifiedEvent);

        var cellsClone = gameBoard.CellsClone;
        notifiedEvent.Cells.ForEach((point, cell) =>
        {
            Assert.AreEqual(cell.Block, cellsClone[point.X, point.Y].Block);
        });
    }

    [Test]
    public void Produce_Next_Tetromino()
    {
        GameBoardEvents notifiedEvent = null;
        gameBoard.GameBoardEventsObservable
                 .Subscribe(x => notifiedEvent = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        Assert.IsFalse(notifiedEvent.TetrominoEvent.HasEvent);

        gameBoard.GoNextState();
        Assert.IsTrue(notifiedEvent.TetrominoEvent.HasEvent);

        var currentTetromino = notifiedEvent.TetrominoEvent.CurrentTetromino;
        // Polyomino of I should has 4 blocks in it
        Assert.AreEqual(4, currentTetromino.Blocks.Length);

        // one to four blocks should have been set after a tetromino is created
        for (int i = 0; i < currentTetromino.Length; i++)
        {
            Assert.AreEqual(i + 1, currentTetromino.Blocks[i].GetNumber());
        }

        var pattern = patterns[0];

        // tetromino should be added to the center of the gameboard.
        // it also counts tetromino size.
        var xPointToPlaceTetromino = gameBoard.Center.X - currentTetromino.Size.Width / 2;

        Assert.AreEqual(xPointToPlaceTetromino, currentTetromino.Position.X);

        // check if the tetromino blocks are placed to the positions it should be.
        for (int i = 0; i < currentTetromino.Length; i++)
        {
            Assert.AreEqual(xPointToPlaceTetromino + pattern[i].X, currentTetromino.Positions[i].X);
            Assert.AreEqual(pattern[i].Y, currentTetromino.Positions[i].Y);
        }
    }

    [Test]
    public void Can_Move_Tetromino_In_The_Cells()
    {
        Tetromino currentTetromino = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => currentTetromino = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        gameBoard.GoNextState();
        var firstTetrominoPos = currentTetromino.Position;

        Assert.IsTrue(gameBoard.MoveLeft());
        Assert.AreEqual(firstTetrominoPos.Subtract(1, 0), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveLeft());
        Assert.AreEqual(firstTetrominoPos.Subtract(2, 0), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveLeft());
        Assert.AreEqual(firstTetrominoPos.Subtract(3, 0), currentTetromino.Position);
        Assert.IsFalse(gameBoard.MoveLeft());
        Assert.AreEqual(firstTetrominoPos.Subtract(3, 0), currentTetromino.Position);

        Assert.IsTrue(gameBoard.MoveRight());
        Assert.AreEqual(firstTetrominoPos.Subtract(2, 0), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveRight());
        Assert.AreEqual(firstTetrominoPos.Subtract(1, 0), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveRight());
        Assert.AreEqual(firstTetrominoPos.Subtract(0, 0), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveRight());
        Assert.AreEqual(firstTetrominoPos.Add(1, 0), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveRight());
        Assert.AreEqual(firstTetrominoPos.Add(2, 0), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveRight());
        Assert.AreEqual(firstTetrominoPos.Add(3, 0), currentTetromino.Position);
        Assert.IsFalse(gameBoard.MoveRight());
        Assert.AreEqual(firstTetrominoPos.Add(3, 0), currentTetromino.Position);

        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 1), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 2), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 3), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 4), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 5), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 6), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 7), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 8), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 9), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 10), currentTetromino.Position);
        Assert.IsFalse(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(3, 10), currentTetromino.Position);
    }

    [Test]
    public void It_Gives_You_a_Direction_Move_From()
    {
        CurrentTetrominoEvent currentTetrominoEvent = null;
        Tetromino             currentTetromino      = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null)
                 .Subscribe(x => 
                 {
                     currentTetrominoEvent = x.TetrominoEvent;
                     currentTetromino = currentTetrominoEvent.CurrentTetromino;
                 })
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        Assert.IsFalse(currentTetrominoEvent.HasEvent);

        gameBoard.GoNextState();
        Assert.IsTrue(currentTetrominoEvent.HasEvent);
        Assert.IsTrue(currentTetrominoEvent.NewTetrominoAdded);
        var firstTetrominoPos = currentTetromino.Position;

        Assert.IsTrue(gameBoard.MoveLeft());
        Assert.AreEqual(Direction.Left, currentTetrominoEvent.TetrominoMoveDirection);

        Assert.IsTrue(gameBoard.MoveRight());
        Assert.AreEqual(Direction.Right, currentTetrominoEvent.TetrominoMoveDirection);

        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(Direction.Down, currentTetrominoEvent.TetrominoMoveDirection);
    }

    [Test]
    public void Cant_Move_Tetromino_On_Other_Block()
    {
        Tetromino currentTetromino = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => currentTetromino = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        var cellsClone = gameBoard.CellsClone;
        cellsClone[3, 13].Set(Block.Create(ThreeSevenBlock.Seven));
        cellsClone[3, 12].Set(Block.Create(ThreeSevenBlock.Five));
        cellsClone[4, 13].Set(Block.Create(ThreeSevenBlock.Five));
        gameBoard.CellsClone = cellsClone;

        gameBoard.GoNextState();
        Point<int> firstTetrominoPos;
        firstTetrominoPos = currentTetromino.Position;

        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 1), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 2), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 3), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 4), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 5), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 6), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 7), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 8), currentTetromino.Position);
        Assert.False(gameBoard.MoveDown());

        GameBoardState state = GameBoardState.Default;
        gameBoard.StateObservable
                 .Subscribe(x => state = x)
                 .AddTo(subscriptions);

        while (state != GameBoardState.OnControlTetromino)
        {
            gameBoard.GoNextState();
        }

        firstTetrominoPos = currentTetromino.Position;

        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 1), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 2), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 3), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 4), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 5), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 6), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 7), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 8), currentTetromino.Position);

        Assert.IsFalse(gameBoard.MoveDown());

    }

    [Test]
    public void Can_Rotate_Tetromino_In_The_Cells()
    {
        Tetromino currentTetromino = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => currentTetromino = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        gameBoard.GoNextState();
        var firstTetrominoPos = currentTetromino.Position;

        Point<int>[] pattern = null;
        Assert.IsTrue(gameBoard.Turn());
        pattern = patterns[1];
        currentTetromino.Foreach((id, point, block) =>
        {
            Assert.AreEqual(point, pattern[id].Add(currentTetromino.Position));
        });

        Assert.IsTrue(gameBoard.Turn());
        pattern = patterns[2];
        currentTetromino.Foreach((id, point, block) =>
        {
            Assert.AreEqual(point, pattern[id].Add(currentTetromino.Position));
        });

        Assert.IsTrue(gameBoard.Turn());
        pattern = patterns[3];
        currentTetromino.Foreach((id, point, block) =>
        {
            Assert.AreEqual(point, pattern[id].Add(currentTetromino.Position));
        });

        Assert.IsTrue(gameBoard.Turn());
        pattern = patterns[0];
        currentTetromino.Foreach((id, point, block) =>
        {
            Assert.AreEqual(point, pattern[id].Add(currentTetromino.Position));
        });
    }

    [Test]
    public void Place_Tetromino_When_Dropped_on_Other_Block()
    {
        Tetromino tetromino = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => tetromino = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        gameBoard.GoNextState();
        var firstTetrominoPos = tetromino.Position;

        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 1), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 2), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 3), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 4), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 5), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 6), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 7), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 8), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 9), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 10), tetromino.Position);

        bool tetrominoCleared = false;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && !x.TetrominoEvent.HasEvent)
                 .Subscribe(x => tetrominoCleared = true)
                 .AddTo(subscriptions);

        PlacedBlockEvent placedBlockEvent = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.PlacedBlockEvent != null)
                 .Select(x => x.PlacedBlockEvent)
                 .Subscribe(x => placedBlockEvent = x);

        Assert.IsFalse(gameBoard.MoveDown());
        Assert.IsTrue(tetrominoCleared);

        Assert.IsNotNull(placedBlockEvent);

        var placedBlocks = placedBlockEvent.placedBlocks;
        Assert.AreEqual(4, placedBlocks.Where(x => x.point.Equals(Point<int>.At(3, 13)))
                                       .First()
                                       .number);
        
        Assert.AreEqual(3, placedBlocks.Where(x => x.point.Equals(Point<int>.At(3, 12)))
                                       .First()
                                       .number);

        Assert.AreEqual(2, placedBlocks.Where(x => x.point.Equals(Point<int>.At(3, 11)))
                                       .First()
                                       .number);

        Assert.AreEqual(1, placedBlocks.Where(x => x.point.Equals(Point<int>.At(3, 10)))
                                       .First()
                                       .number);
    }

    [Test]
    public void Drop_All_Blocks_Above_Ground()
    {
        BlockMoveEvent blockMoveEvent = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.BlockMoveEvent.HasEvent)
                 .Select(x => x.BlockMoveEvent)
                 .Subscribe(x => blockMoveEvent = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();

        Cell[,] cells;
        cells = gameBoard.CellsClone;
        cells[3, 1].Set(Block.Create(ThreeSevenBlock.Seven));
        cells[3, 2].Set(Block.Create(ThreeSevenBlock.Five));
        cells[4, 1].Set(Block.Create(ThreeSevenBlock.Three));
        gameBoard.CellsClone = cells;

        Assert.IsNull(blockMoveEvent);

        gameBoard.DropAllBlocks();

        TwoDimensionalMovement[] movements;
        movements = blockMoveEvent.movements;
        Assert.AreEqual(movements.Length, 3);

        // block at (4,1) should be dropped to (4,13) in this pattern
        // (3,2) to (3,13) 
        // (3,1) to (3,12) 
        Assert.IsTrue(movements.Where(x => x.source.Equals(Point<int>.At(4, 1)))
                               .Select(x => x)
                               .FirstOrDefault()
                               .destination.Equals(Point<int>.At(4, 13)));

        Assert.IsTrue(movements.Where(x => x.source.Equals(Point<int>.At(3, 2)))
                               .Select(x => x)
                               .FirstOrDefault()
                               .destination.Equals(Point<int>.At(3, 13)));

        Assert.IsTrue(movements.Where(x => x.source.Equals(Point<int>.At(3, 1)))
                               .Select(x => x)
                               .FirstOrDefault()
                               .destination.Equals(Point<int>.At(3, 12)));

        cells = gameBoard.CellsClone;

        Assert.AreEqual(7, cells[3, 12].Block.GetNumber());
        Assert.AreEqual(5, cells[3, 13].Block.GetNumber());
        Assert.AreEqual(3, cells[4, 13].Block.GetNumber());

        cells[3, 1].Set(Block.Create(ThreeSevenBlock.One));
        cells[3, 2].Set(Block.Create(ThreeSevenBlock.Six));
        cells[4, 1].Set(Block.Create(ThreeSevenBlock.Six));
        cells[4, 2].Set(Block.Create(ThreeSevenBlock.Seven));
        gameBoard.CellsClone = cells;

        gameBoard.DropAllBlocks();

        Assert.IsNotNull(blockMoveEvent);

        cells = gameBoard.CellsClone;

        Assert.AreEqual(7, cells[3, 12].Block.GetNumber());
        Assert.AreEqual(5, cells[3, 13].Block.GetNumber());
        Assert.AreEqual(3, cells[4, 13].Block.GetNumber());
        Assert.AreEqual(1, cells[3, 10].Block.GetNumber());
        Assert.AreEqual(6, cells[3, 11].Block.GetNumber());
        Assert.AreEqual(6, cells[4, 11].Block.GetNumber());
        Assert.AreEqual(7, cells[4, 12].Block.GetNumber());

        movements = blockMoveEvent.movements;
        Assert.AreEqual(movements.Length, 4);

        // block at (4,2) should be dropped to (4,12) in this pattern
        // (4,1) to (4,11) 
        // (3,2) to (3,11) 
        // (3,1) to (3,10) 

        Assert.IsTrue(movements.Where(x => x.source.Equals(Point<int>.At(4, 2)))
                               .Select(x => x)
                               .FirstOrDefault()
                               .destination.Equals(Point<int>.At(4, 12)));

        Assert.IsTrue(movements.Where(x => x.source.Equals(Point<int>.At(4, 1)))
                               .Select(x => x)
                               .FirstOrDefault()
                               .destination.Equals(Point<int>.At(4, 11)));

        Assert.IsTrue(movements.Where(x => x.source.Equals(Point<int>.At(3, 2)))
                               .Select(x => x)
                               .FirstOrDefault()
                               .destination.Equals(Point<int>.At(3, 11)));

        Assert.IsTrue(movements.Where(x => x.source.Equals(Point<int>.At(3, 1)))
                               .Select(x => x)
                               .FirstOrDefault()
                               .destination.Equals(Point<int>.At(3, 10)));
    }
    
    [Test]
    public void Should_Notify_Deleted_Blocks()
    {
        GameBoardState state = GameBoardState.Default;
        DeletedBlockEvent deletedBlockEvent = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.DeletedBlockEvent != null)
                 .Select(x => x.DeletedBlockEvent)
                 .Subscribe(x => deletedBlockEvent = x)
                 .AddTo(subscriptions);

        gameBoard.StateObservable
                 .Subscribe(x => state = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        gameBoard.GoNextState();

        Assert.IsTrue(gameBoard.MoveLeft());
        Assert.IsTrue(gameBoard.MoveLeft());
        Assert.IsTrue(gameBoard.MoveLeft());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.IsTrue(gameBoard.MoveDown());

        Assert.IsNull(deletedBlockEvent);

        Assert.IsFalse(gameBoard.MoveDown());
        Assert.AreEqual(GameBoardState.BeforeDropBlocks, state);
        gameBoard.GoNextState();
        Assert.IsNull(deletedBlockEvent);
        Assert.AreEqual(GameBoardState.BeforeDeleteBlocks, state);

        gameBoard.GoNextState();
        Assert.AreEqual(GameBoardState.BeforeDropBlocks, state);

        Assert.IsNotNull(deletedBlockEvent);

        Assert.IsTrue(deletedBlockEvent.deletedBlocks.Select(x => x.point).Contains(Point<int>.At(0, 12)));
        Assert.IsTrue(deletedBlockEvent.deletedBlocks.Select(x => x.point).Contains(Point<int>.At(0, 13)));

        Assert.AreEqual(3, deletedBlockEvent.deletedBlocks.Where(x => x.point.Equals(Point<int>.At(0, 12))).Select(x => x.number).First());
        Assert.AreEqual(4, deletedBlockEvent.deletedBlocks.Where(x => x.point.Equals(Point<int>.At(0, 13))).Select(x => x.number).First());
    }

    [Test]
    public void StateTest()
    {
        Tetromino tetromino = null;
        gameBoard.GameBoardEventsObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => tetromino = x)
                 .AddTo(subscriptions);

        GameBoardState state = GameBoardState.Default;
        gameBoard.StateObservable
                 .Subscribe(x => state = x)
                 .AddTo(subscriptions);
        Assert.AreEqual(GameBoardState.Default, state);

        gameBoard.StartGame();
        Assert.AreEqual(GameBoardState.Default, state);

        gameBoard.GoNextState();
        Assert.AreEqual(GameBoardState.OnControlTetromino, state);

        var firstTetrominoPos = tetromino.Position;

        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 1), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 2), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 3), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 4), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 5), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 6), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 7), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 8), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 9), tetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 10), tetromino.Position);

        Assert.IsFalse(gameBoard.MoveDown());
        Assert.AreEqual(GameBoardState.BeforeDropBlocks, state);

        gameBoard.GoNextState();
        Assert.AreEqual(GameBoardState.BeforeDeleteBlocks, state);

        // DeleteBlocks happens here so it will go back to BeforeDropBlocks state
        gameBoard.GoNextState();
        Assert.AreEqual(GameBoardState.BeforeDropBlocks, state);

        gameBoard.GoNextState();
        Assert.AreEqual(GameBoardState.BeforeDeleteBlocks, state);

        gameBoard.GoNextState();
        Assert.AreEqual(GameBoardState.BeforeAddTetromino, state);

        gameBoard.GoNextState();
        Assert.AreEqual(GameBoardState.OnControlTetromino, state);
    }
}
