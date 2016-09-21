using System.Collections;
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

public class GameBoardEvents
{
    public bool IsReady { get { return Cells != null; } }

    public Cell[,]               Cells                = null;
    public CurrentTetrominoEvent TetrominoEvent       = null;
    public PlacedBlockEvent      PlacedBlockEvent     = null;
    public DeletedBlockEvent     DeletedBlockEvent    = null;
    public BlockMoveEvent        BlockMoveEvent       = null;
}

public class CurrentTetrominoEvent
{
    public bool HasEvent { get { return CurrentTetromino != null; } }
    public bool NewTetrominoAdded { get; set; }
    public bool TetrominoIsPlaced { get; set; }

    public Tetromino    CurrentTetromino = null;
    public Point<int>[] CurrentTetrominoPositions { get { return CurrentTetromino.Positions; } }
    public IBlock[]     CurrentTetrominoBlocks    { get { return CurrentTetromino.Blocks; } }
}

public struct PlacedBlock
{
    public IBlock block;
    public Point<int> point;
}

public class PlacedBlockEvent
{
    public PlacedBlock[] placedBlocks;
}

public class BlockMoveEvent
{
    public bool HasEvent { get { return movements != null; } }
    public TwoDimensionalMovement[] movements = null;
}

public class DeletedBlockEvent
{
    public DeletedBlock[] deletedBlocks;
}

public class DeletedBlock
{
    public int number;
    public ThreeSevenBlock type;
    public Point<int> point;
}

public class GameBoard : CellBoard, IGameBoardObservable
{
    public IObservable<GameBoardState> StateObservable { get { return _stateReactiveProperty.AsObservable(); } }
    private ReactiveProperty<GameBoardState> _stateReactiveProperty = new ReactiveProperty<GameBoardState>(GameBoardState.Default);

    public  IObservable<GameBoardEvents> GameBoardObservable { get { return _gameBoardStream.AsObservable(); } }
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
        //place NextTetromino on Center
        NextTetromino.Position = new Point<int> { X = Center.X - NextTetromino.Size.Width / 2, Y = 0 };
        _currentTetromino = NextTetromino;
        _nextTetrominos.Remove(NextTetromino);
        _newTetrominoAdded = true;

        PrepareNextTetromino();
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

        UpdateGameBoard();
    }

    private GameBoardEvents             _gameboardEvents  = null;
    private PlacedBlockEvent            _placedBlockEvent = null;
    private TwoDimensionalMovement[]    _blockMovements   = null;
    private DeletedBlockEvent           _deletedBlockEvent = null;
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

        _gameboardEvents.TetrominoEvent = new CurrentTetrominoEvent()
        { CurrentTetromino  = this._currentTetromino,
          NewTetrominoAdded = this._newTetrominoAdded,
          TetrominoIsPlaced = this._tetrominoIsPlaced};
        _newTetrominoAdded = false;
        _tetrominoIsPlaced = false;

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

        _stateReactiveProperty.Value = GameBoardState.BeforeDropBlocks;
    }

    public void DropAllBlocks()
    {
        _blockMovements = GetPositionsToDropObjects();
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
    }

    // this method returns an array which is same sized for the Cells
    // and stores Point used for blocks where to move
    // for example if (5, 10) in the array has a Point(5, 12)
    // meand the block at (5, 10) will move to (5, 12) 
    private TwoDimensionalMovement[] GetPositionsToDropObjects()
    {
        bool[,] isEmptyCell = IsEmptyCell;

        List<TwoDimensionalMovement> objectMovements = new List<TwoDimensionalMovement>(); 
        //TwoDimensionalMovements objectMovements = new TwoDimensionalMovements(Size);

        for(int y = Size.Height - 1; y >= 0; y--)
        {
            for(int x = 0; x < Size.Width; x++)
            {
                if (isEmptyCell[x, y]) continue;

                int i = 0;
                int verticalMove = 0;
                while(true)
                {
                    i++;

                    // add movement when the space below the block is empty to know
                    // how many times drops the block
                    if (isEmptyCell.IsOutOfRange(Point<int>.At(x, y + i)) ||
                       !isEmptyCell[x, y + i])
                    {
                        break;
                    }

                    verticalMove++;
                }

                if (verticalMove == 0) continue;

                var movement = new TwoDimensionalMovement(Point<int>.At(x, y), Point<int>.At(x, y + verticalMove));
                objectMovements.Add(movement);
                isEmptyCell.Swap(movement);
            }
        }

        return objectMovements.ToArray();
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

        var placedBlocks = new List<PlacedBlock>();

        _currentTetromino.Foreach((point, block) => 
        {
            Cells[point.X, point.Y].Set(block);
            placedBlocks.Add(new PlacedBlock() { block = block, point = point });
        });

        _placedBlockEvent = new PlacedBlockEvent() { placedBlocks = placedBlocks.ToArray() };
        _currentTetromino = null;
        _tetrominoIsPlaced = true;
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
