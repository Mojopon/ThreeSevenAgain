using System.Collections;
using ThreeSeven.Model;
using System;

public class GameBoard : MaskedCellBoard
{
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

    public GameBoard(Size<int> size) : base(size) { }

    public void Start()
    {
        Clear();
        NextTetromino = new Tetromino(Polyomino.Create(PolyominoIndex.I), () => Block.Create());

        currentTetromino = NextTetromino;
    }

    private bool CanPlaceCurrentTetromino()
    {
        var success = true;
        currentTetromino.Foreach((point, block) =>
        {
            success = Cells[point.X, point.Y].Block. ? success : false;
        });

        return success;
    }

    private bool PlaceTetromino()
    {
        var cells = CellsClone;
        var canPlace = true;

        currentTetromino.Foreach((point, block) =>
        {
            if(cells[point.X, point.Y] == null)
            {

            }
        });


        return true;
    }

    public override void Clear()
    {
        base.Clear();
    }
}
