using System.Collections;
using ThreeSeven.Model;
using System;
using UniRx;

public class GameBoard : MaskedCellBoard, IGameBoardObservable
{
    public IObservable<Cell[,]> GameBoardCellsObservable { get { return _gameBoardCellsStream.AsObservable(); } }
    private ISubject<Cell[,]> _gameBoardCellsStream = new BehaviorSubject<Cell[,]>(null);

    private Random random = new Random();

    private Tetromino currentTetromino;

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
            currentTetromino.Foreach((point, block) => 
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
        _gameBoardCellsStream.OnNext(ActualCells);

        Start();
    }

    public void Start()
    {
        Clear();
        NextTetromino = new Tetromino(Polyomino.Create(), () => Block.Create());
        NextTetromino.Position = new Point<int> { X = Center.X - NextTetromino.Size.Width / 2, Y = 0 };

        currentTetromino = NextTetromino;
    }

    public bool MoveLeft()
    {
        return MoveCurrentTetrominoTo(new Point<int> { X = currentTetromino.Position.X - 1, Y = currentTetromino.Position.Y });
    }

    public bool MoveRight()
    {
        return MoveCurrentTetrominoTo(new Point<int> { X = currentTetromino.Position.X + 1, Y = currentTetromino.Position.Y });
    }

    public bool MoveDown()
    {
        return MoveCurrentTetrominoTo(new Point<int> { X = currentTetromino.Position.X, Y = currentTetromino.Position.Y + 1 });
    }

    private bool MoveCurrentTetrominoTo(Point<int> newPosition)
    {
        if (CanMoveCurrentTetrominoTo(newPosition))
        {
            currentTetromino.Position = newPosition;
            UpdateGameBoard();
            return true;
        }

        return false;
    }

    private void UpdateGameBoard()
    {
        _gameBoardCellsStream.OnNext(ActualCells);
    }

    private bool CanMoveCurrentTetrominoTo(Point<int> newPosition)
    {
        var oldPosition = currentTetromino.Position;
        bool canMove = false;

        currentTetromino.Position = newPosition;
        if(CanPlaceCurrentTetromino())
        {
            canMove = true;
        }

        currentTetromino.Position = oldPosition;
        return canMove;
    }

    private bool CanPlaceCurrentTetromino()
    {
        var success = true;
        currentTetromino.Foreach((point, block) => 
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

    private bool PlaceTetromino()
    {
        if (!CanPlaceCurrentTetromino()) return false;

        currentTetromino.Foreach((point, block) => Cells[point.X, point.Y].Set(block));
        return true;
    }

    public override void Clear()
    {
        base.Clear();
    }
}
