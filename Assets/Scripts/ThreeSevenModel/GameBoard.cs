using System.Collections;
using ThreeSeven.Model;
using System;

public class GameBoard : MaskedCellBoard
{
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
            currentTetromino.Foreach((point, block) =>  cells[point.X, point.Y].Set(block));
            return cells;
        }
    }


    public GameBoard(Size<int> size) : base(size)
    {
        Start();
    }

    public void Start()
    {
        Clear();
        NextTetromino = new Tetromino(Polyomino.Create(), () => Block.Create());
        NextTetromino.Position = new Point<int> { X = Center.X - NextTetromino.Size.Width / 2, Y = 0 };

        currentTetromino = NextTetromino;
    }

    private bool CanPlaceCurrentTetromino()
    {
        var success = true;
        currentTetromino.Foreach((point, block) => success = Cells[point.X, point.Y].IsNull ? success : false);

        return success;
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
