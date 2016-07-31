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

    }

    private void PlaceTetromino()
    {
        for(int i = 0; i < NextTetromino.Length; i++)
        {

        }
    }

    public override void Clear()
    {
        base.Clear();
    }
}
