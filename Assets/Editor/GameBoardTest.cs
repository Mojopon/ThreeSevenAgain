using System.Collections;
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
        gameBoard.GameBoardObservable
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
        gameBoard.GameBoardObservable
                 .Subscribe(x => notifiedEvent = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        Assert.IsFalse(notifiedEvent.TetrominoEvent.HasEvent);

        gameBoard.AddNextTetromino();
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
        gameBoard.GameBoardObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => currentTetromino = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        gameBoard.AddNextTetromino();
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
    public void Cant_Move_Tetromino_On_Other_Block()
    {
        Tetromino currentTetromino = null;
        gameBoard.GameBoardObservable
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

        gameBoard.AddNextTetromino();
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

        gameBoard.AddNextTetromino();
        firstTetrominoPos = currentTetromino.Position;


        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 1), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 2), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 3), currentTetromino.Position);
        Assert.IsTrue(gameBoard.MoveDown());
        Assert.AreEqual(firstTetrominoPos.Add(0, 4), currentTetromino.Position);
        Assert.IsFalse(gameBoard.MoveDown());

        //Debug.Log(gameBoard);
    }

    [Test]
    public void Can_Rotate_Tetromino_In_The_Cells()
    {
        Tetromino currentTetromino = null;
        gameBoard.GameBoardObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => currentTetromino = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        gameBoard.AddNextTetromino();
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
        gameBoard.GameBoardObservable
                 .Where(x => x != null && x.TetrominoEvent.HasEvent)
                 .Select(x => x.TetrominoEvent.CurrentTetromino)
                 .Subscribe(x => tetromino = x)
                 .AddTo(subscriptions);

        gameBoard.StartGame();
        gameBoard.AddNextTetromino();
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
        gameBoard.GameBoardObservable
                 .Where(x => x != null && !x.TetrominoEvent.HasEvent)
                 .Subscribe(x => tetrominoCleared = true)
                 .AddTo(subscriptions);

        Assert.IsFalse(gameBoard.MoveDown());
        Assert.IsTrue(tetrominoCleared);
    }

    [Test]
    public void Drop_All_Blocks_Above_Ground()
    {
        BlockMoveEvent blockMoveEvent = null;
        gameBoard.GameBoardObservable
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
}
