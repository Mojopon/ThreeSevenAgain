using System.Collections;
using ThreeSeven.Model;
using System;
using UniRx;

public class GameBoard : MaskedCellBoard, IGameBoardObservable
{
    public IObservable<Cell[,]> GameBoardCellsObservable { get { return _gameBoardCellsStream.AsObservable(); } }
    private ISubject<Cell[,]> _gameBoardCellsStream = new BehaviorSubject<Cell[,]>(null);

    private Func<Tetromino> _createTetromino = new TetrominoFactory().Create;

    private Tetromino _currentTetromino;

    private Tetromino _nextTetromino;
    public Tetromino NextTetromino
    {
        get { return _nextTetromino; }
        private set
        {
            if(value != _nextTetromino)
            {
                _nextTetromino = value;
            }
        }
    }

    public override Cell[,] ActualCells
    {
        get
        {
            var cells = base.ActualCells;
            _currentTetromino.Foreach((point, block) => 
            {
                var actualPoint = new Point<int> { X = point.X, Y = point.Y - TopMask };

                if (!IsOutOfRange(actualPoint))
                {
                    cells[actualPoint.X, actualPoint.Y].Set(block);
                }
            });
            return cells;
        }
    }


    public GameBoard(Size<int> size) : base(size)
    {
    }

    public void SetTetrominoFactory(ITetrominoFactory factory)
    {
        _createTetromino = factory.Create;
    }

    public void Start()
    {
        Clear();
        PrepareNextTetromino();
    }

    private void PrepareNextTetromino()
    {
        NextTetromino = _createTetromino();
        NextTetromino.Position = new Point<int> { X = Center.X - NextTetromino.Size.Width / 2, Y = 0 };

        _currentTetromino = NextTetromino;

        UpdateGameBoardCellsObservable();
    }

    private void UpdateGameBoardCellsObservable()
    {
        _gameBoardCellsStream.OnNext(ActualCells);
    }

    public bool MoveLeft()
    {
        return MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X - 1, Y = _currentTetromino.Position.Y });
    }

    public bool MoveRight()
    {
        return MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X + 1, Y = _currentTetromino.Position.Y });
    }

    public bool MoveDown()
    {
        return MoveCurrentTetrominoTo(new Point<int> { X = _currentTetromino.Position.X, Y = _currentTetromino.Position.Y + 1 });
    }

    public void Place()
    {
        if(CanPlaceCurrentTetromino())
        {
            PlaceCurrentTetromino();
            PrepareNextTetromino();
        }
    }

    private bool MoveCurrentTetrominoTo(Point<int> newPosition)
    {
        if (CanMoveCurrentTetrominoTo(newPosition))
        {
            _currentTetromino.Position = newPosition;
            UpdateGameBoardCellsObservable();
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
            UpdateGameBoardCellsObservable();
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
        return true;
    }

    public override void Clear()
    {
        base.Clear();
    }
}
