using System.Collections;
using ThreeSeven.Model;
using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;

public enum GameBoardState
{
    Paused,
    BeforeAddTetromino,
    OnControlTetromino,
    BeforeDropBlocks,
    BeforeDeleteBlocks,
}

public class GameBoardEvents
{
    public bool IsReady { get { return Cells != null; } }

    public Cell[,] Cells = null;
    public CurrentTetrominoEvent TetrominoEvent       = null;
    public PlacedTetrominoEvent  PlacedTetrominoEvent = null;
}

public class CurrentTetrominoEvent
{
    public bool HasEvent { get { return CurrentTetromino != null; } }

    public Tetromino    CurrentTetromino = null;
    public Point<int>[] CurrentTetrominoPositions { get { return CurrentTetromino.Positions; } }
    public IBlock[]     CurrentTetrominoBlocks    { get { return CurrentTetromino.Blocks; } }
}

public class PlacedTetrominoEvent
{
    public Tetromino PlacedTetromino;
}

public class GameBoard : CellBoard, IGameBoardObservable
{

    public  IObservable<GameBoardEvents> GameBoardObservable { get { return _gameBoardStream.AsObservable(); } }
    private ISubject<GameBoardEvents> _gameBoardStream = new BehaviorSubject<GameBoardEvents>(null);

    private Func<Tetromino> _createTetromino = new TetrominoFactory().Create;

    private Tetromino _currentTetromino;

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
    }

    public void AddNextTetromino()
    {
        //place NextTetromino on Center
        NextTetromino.Position = new Point<int> { X = Center.X - NextTetromino.Size.Width / 2, Y = 0 };
        _currentTetromino = NextTetromino;
        _nextTetrominos.Remove(NextTetromino);

        PrepareNextTetromino();
        UpdateGameBoard();
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

        UpdateGameBoard();
    }

    private GameBoardEvents _gameboardEvents = null;
    private Tetromino _placedTetromino = null;
    // UpdateGameBoard will notify that the board is updated to all its subscribers
    private void UpdateGameBoard()
    {
        _gameboardEvents = new GameBoardEvents();
        _gameboardEvents.Cells = this.CellsClone;

        if(_placedTetromino != null)
        {
            _gameboardEvents.PlacedTetrominoEvent = new PlacedTetrominoEvent(){ PlacedTetromino = _placedTetromino };
            _placedTetromino = null;
        }

        _gameboardEvents.TetrominoEvent = new CurrentTetrominoEvent()

        {
            CurrentTetromino = this._currentTetromino,
        };

        if (_gameboardEvents.Cells != null)
        {
            _gameBoardStream.OnNext(_gameboardEvents);
            _gameboardEvents = null;
        }
    }

    public bool MoveLeft()
    {
        if (_currentTetromino == null) return false;

        return MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X - 1, Y = _currentTetromino.Position.Y });
    }

    public bool MoveRight()
    {
        if (_currentTetromino == null) return false;

        return MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X + 1, Y = _currentTetromino.Position.Y });
    }

    public bool MoveDown()
    {
        if (_currentTetromino == null) return false;

        bool moveSucceed = MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X, Y = _currentTetromino.Position.Y + 1 });

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
    }

    public void DropAllBlocks()
    {
        var afterObjectsDroppedPositions = GetPositionsToDropObjects();

        afterObjectsDroppedPositions.ForEachFromBottomToTop((source, destination) =>
        {
            Cells.Swap(source, destination);
        });

        UpdateGameBoard();
    }
    
    
    // this method returns an array which is same sized for the Cells
    // and stores Point used for blocks where to move
    // for example if (5, 10) in the array has a Point(5, 12)
    // meand the block at (5, 10) will move to (5, 12) 
    private Point<int>[,] GetPositionsToDropObjects()
    {
        bool[,] isEmptyCell = IsEmptyCell;

        Point<int>[,] cellObjectMovement = new Point<int>[Size.Width,Size.Height];

        for(int y = Size.Height - 1; y >= 0; y--)
        {
            for(int x = 0; x < Size.Width; x++)
            {
                cellObjectMovement[x, y] = Point<int>.At(x, y);
                if (isEmptyCell[x, y]) continue;

                int i = 0;
                int movement = 0;
                while(true)
                {
                    i++;

                    if (isEmptyCell.IsOutOfRange(Point<int>.At(x, y + i)) ||
                       !isEmptyCell[x, y + i])
                    {
                        break;
                    }

                    movement++;
                }
                var target = Point<int>.At(x, y + movement);

                Debug.Log("Move " + Point<int>.At(x, y) + " to " + target);

                cellObjectMovement[x, y] = target;
                isEmptyCell.Swap(Point<int>.At(x, y), target);
            }
        }

        return cellObjectMovement;
    }

    private bool MoveCurrentTetrominoTo(Point<int> newPosition)
    {
        if (CanMoveCurrentTetrominoTo(newPosition))
        {
            _currentTetromino.Position = newPosition;
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

        _currentTetromino.Foreach((point, block) => Cells[point.X, point.Y].Set(block));

        _placedTetromino = _currentTetromino;
        _currentTetromino = null;
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
