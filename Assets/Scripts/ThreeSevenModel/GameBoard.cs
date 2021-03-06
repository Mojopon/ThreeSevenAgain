﻿using System.Collections;
using ThreeSeven.Model;
using ThreeSeven.Helper;
using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameBoardState
{
    Default,
    Paused,
    BeforeAddTetromino,
    OnControlTetromino,
    BeforeDropBlocks,
    BeforeDeleteBlocks,
}

public class GameBoard : CellBoard, IGameBoardObservable
{
    public IObservable<GameBoardState> StateObservable { get { return _stateReactiveProperty.AsObservable(); } }
    private ReactiveProperty<GameBoardState> _stateReactiveProperty = new ReactiveProperty<GameBoardState>(GameBoardState.Default);

    public  IObservable<GameBoardEvents> GameBoardEventsObservable { get { return _gameBoardStream.AsObservable(); } }
    private ISubject<GameBoardEvents> _gameBoardStream = new BehaviorSubject<GameBoardEvents>(null);

    private Func<Tetromino> _createTetromino = new TetrominoFactory().Create;

    public int NumberOfNextTetrominos = 1;

    private List<Tetromino> _nextTetrominos = null;
    public Tetromino NextTetromino
    {
        get
        {
            if(_nextTetrominos == null || _nextTetrominos.Count == 0)
            {
                return null;
            }

            return _nextTetrominos[0];
        }
    }
    public Tetromino[] NextTetrominos { get { return _nextTetrominos.ToArray(); } }


    public GameBoard(Size<int> size) : base(size)
    {
    }

    public void SetTetrominoFactory(ITetrominoFactory factory)
    {
        _createTetromino = factory.Create;
    }

    public void StartGame()
    {
        Clear();

        _nextTetrominos = null;

        PrepareNextTetromino();

        UpdateGameBoard();
    }

    public void GoNextState()
    {
        switch(_stateReactiveProperty.Value)
        {
            case GameBoardState.Default:
            case GameBoardState.BeforeAddTetromino:
                AddNextTetromino();
                break;
            case GameBoardState.BeforeDropBlocks:
                DropAllBlocks();
                break;
            case GameBoardState.BeforeDeleteBlocks:
                Resolve();
                break;
        }
    }

    private Tetromino _currentTetromino;
    private bool _newTetrominoAdded = false;
    private bool _tetrominoIsPlaced = false;
    private void AddNextTetromino()
    {
        if (NextTetromino == null) return;

        //place NextTetromino on Center
        NextTetromino.Position = new Point<int> { X = Center.X - NextTetromino.Size.Width / 2, Y = 0 };
        _currentTetromino = NextTetromino;
        _nextTetrominos.Remove(NextTetromino);

        PrepareNextTetromino();

        _currentTetrominoEvent = new CurrentTetrominoEvent()
        {
            CurrentTetromino = _currentTetromino,
            NewTetrominoAdded = true,
        };

        UpdateGameBoard();

        _stateReactiveProperty.Value = GameBoardState.OnControlTetromino;
    }

    private void PrepareNextTetromino()
    {
        if (_nextTetrominos == null)
        {
            _nextTetrominos = new List<Tetromino>();

        }

        int c = 0;
        while (NumberOfNextTetrominos > _nextTetrominos.Count)
        {
            _nextTetrominos.Add(_createTetromino());

            c++;
            if(c > 1000)
            {
                throw new Exception("Something weird happend in PrepareNextTetromino");
            }
        }

        //UpdateGameBoard();
    }

    private GameBoardEvents             _gameboardEvents         = null;
    private PlacedBlockEvent            _placedBlockEvent        = null;
    private CurrentTetrominoEvent       _currentTetrominoEvent   = null;
    private TwoDimensionalMovement[]    _blockMovements          = null;
    private DeletedBlockEvent           _deletedBlockEvent       = null;
    // UpdateGameBoard will notify that the board is updated to all its subscribers
    private void UpdateGameBoard()
    {
        _gameboardEvents = new GameBoardEvents();
        _gameboardEvents.Cells = this.CellsClone;

        if(_placedBlockEvent != null)
        {
            _gameboardEvents.PlacedBlockEvent = _placedBlockEvent;
            _placedBlockEvent = null;
        }

        if(_deletedBlockEvent != null)
        {
            _gameboardEvents.DeletedBlockEvent = _deletedBlockEvent;
            _deletedBlockEvent = null;
        }

        if(_currentTetrominoEvent != null)
        {
            _gameboardEvents.TetrominoEvent = _currentTetrominoEvent;
            _currentTetrominoEvent = null;
        }
        else
        {
            _gameboardEvents.TetrominoEvent = new CurrentTetrominoEvent()
            {
                CurrentTetromino = _currentTetromino
            };
        }

        /*
        _gameboardEvents.TetrominoEvent = new CurrentTetrominoEvent()
        { CurrentTetromino  = this._currentTetromino,
          NewTetrominoAdded = this._newTetrominoAdded,
          TetrominoIsPlaced = this._tetrominoIsPlaced};
        _newTetrominoAdded = false;
        _tetrominoIsPlaced = false;

        */

        _gameboardEvents.BlockMoveEvent = new BlockMoveEvent()
        { movements = _blockMovements };
        _blockMovements = null;

        if (_gameboardEvents.Cells != null)
        {
            _gameBoardStream.OnNext(_gameboardEvents);
            _gameboardEvents = null;
        }
    }

    public bool MoveLeft()
    {
        if (_currentTetromino == null) return false;

        return MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X - 1, Y = _currentTetromino.Position.Y },
               Direction.Left);
    }

    public bool MoveRight()
    {
        if (_currentTetromino == null) return false;

        return MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X + 1, Y = _currentTetromino.Position.Y },
               Direction.Right);
    }

    public bool MoveDown()
    {
        if (_currentTetromino == null) return false;

        bool moveSucceed = MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X, Y = _currentTetromino.Position.Y + 1 },
                           Direction.Down);

        if (!moveSucceed)
        {
            Place();
        }

        return moveSucceed;
    }

    private void Place()
    {
        if(CanPlaceCurrentTetromino())
        {
            PlaceCurrentTetromino();
            UpdateGameBoard();
            //PrepareNextTetromino();
        }

        _stateReactiveProperty.Value = GameBoardState.BeforeDropBlocks;
    }

    public void DropAllBlocks()
    {
        _blockMovements = Cells.CellsToBoolGrid().DropThreeSevenGrid();
        _blockMovements.ForEach((movement) =>
        {
            Cells.Swap(movement);
        });

        UpdateGameBoard();

        _stateReactiveProperty.Value = GameBoardState.BeforeDeleteBlocks;
    }

    private void Resolve()
    {
        var pointsToDeleteBlocks = Cells.CellsToNumberGrid().ResolveThreeSevenGrid();

        if (pointsToDeleteBlocks.Length == 0)
        {
            _stateReactiveProperty.Value = GameBoardState.BeforeAddTetromino;
            return;
        }
        else
        {
            DeleteBlocks(pointsToDeleteBlocks);
            _stateReactiveProperty.Value = GameBoardState.BeforeDropBlocks;
        }
    }

    private void DeleteBlocks(Point<int>[] points)
    {
        var _deletedBlockList = new List<DeletedBlock>();

        points.ForEach(point =>
        {
            var block = Cells[point.X, point.Y].Block;
            var deletedBlock = new DeletedBlock()
            {
                number = block.GetNumber(),
                type = block.Type,
                point = point,
            };
            _deletedBlockList.Add(deletedBlock);

            Cells[point.X, point.Y].Clear();
        });

        _deletedBlockEvent = new DeletedBlockEvent()
        { deletedBlocks = _deletedBlockList.ToArray() };

        UpdateGameBoard();
    }

    private bool MoveCurrentTetrominoTo(Point<int> newPosition, Direction direction)
    {
        if (CanMoveCurrentTetrominoTo(newPosition))
        {
            _currentTetromino.Position = newPosition;

            _currentTetrominoEvent = new CurrentTetrominoEvent()
            {
                CurrentTetromino = _currentTetromino,
                TetrominoMoveDirection = direction
            };
            UpdateGameBoard();
            return true;
        }

        return false;
    }

    private bool CanMoveCurrentTetrominoTo(Point<int> newPosition)
    {
        var oldPosition = _currentTetromino.Position;
        bool canMove = false;

        _currentTetromino.Position = newPosition;
        if(CanPlaceCurrentTetromino())
        {
            canMove = true;
        }

        _currentTetromino.Position = oldPosition;
        return canMove;
    }

    private bool CanPlaceCurrentTetromino()
    {
        var success = true;
        _currentTetromino.Foreach((point, block) => 
        {
            if (IsOutOfRange(point))
            {
                success = false;
            }
            else
            {
                success = Cells[point.X, point.Y].IsNull ? success : false;
            }
        });

        return success;
    }

    private bool IsOutOfRange(Point<int> point)
    {
        return 0 > point.X || 0 > point.Y || point.X >= Size.Width || point.Y >= Size.Height;
    }

    public bool Turn() { return Turn(true); }

    public bool Turn(bool clockowise)
    {
        return TurnCurrentTetromino(clockowise);
    }

    private bool TurnCurrentTetromino(bool clockowise)
    {
        if(CanTurnCurrentTetromino(clockowise))
        {
            _currentTetromino.Turn(clockowise);
            UpdateGameBoard();
            return true;
        }

        return false;
    }

    private bool CanTurnCurrentTetromino(bool clockowise)
    {
        bool canMove = false;
        _currentTetromino.Turn(clockowise);

        if (CanPlaceCurrentTetromino())
        {
            canMove = true;
        }

        _currentTetromino.Turn(!clockowise);

        return canMove;
    }

    private bool PlaceCurrentTetromino()
    {
        if (!CanPlaceCurrentTetromino()) return false;

        var placedBlocks = new List<PlacedBlock>();

        _currentTetromino.Foreach((point, block) => 
        {
            Cells[point.X, point.Y].Set(block);
            placedBlocks.Add(new PlacedBlock()
            {
                type   = block.Type,
                number = block.GetNumber(),
                point  = point });
        });

        _placedBlockEvent = new PlacedBlockEvent() { placedBlocks = placedBlocks.ToArray() };
        _currentTetromino = null;

        _currentTetrominoEvent = new CurrentTetrominoEvent()
        {
            TetrominoIsPlaced = true,
        };

        return true;
    }

    public override void Clear()
    {
        base.Clear();
    }

    public override string ToString()
    {
        int[,] boardNumbers = new int[this.Size.Width, this.Size.Height];

        Cells.ForEach((point, cell) =>
        {
            boardNumbers[point.X, point.Y] = cell.Block.GetNumber();
        });

        _currentTetromino.Foreach((point, block) =>
        {
            boardNumbers[point.X, point.Y] = block.GetNumber();
        });

        string str = "";

        for (int y = 0; y < Size.Height; y++)
        {
            for (int x = 0; x < Size.Width; x++)
            {
                str += boardNumbers[x, y].ToString();
            }

            str += "\n";
        }

        return str;
    }
}
