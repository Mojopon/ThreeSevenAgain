using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public class GameBoardEvents
{
    public bool IsReady { get { return Cells != null; } }

    public Cell[,] Cells = null;
    public CurrentTetrominoEvent TetrominoEvent    = null;
    public PlacedBlockEvent      PlacedBlockEvent  = null;
    public DeletedBlockEvent     DeletedBlockEvent = null;
    public BlockMoveEvent        BlockMoveEvent    = null;
}

public class CurrentTetrominoEvent
{
    public bool HasEvent { get { return CurrentTetromino != null; } }
    public bool NewTetrominoAdded { get; set; }
    public bool TetrominoIsPlaced { get; set; }

    public Tetromino CurrentTetromino = null;
    public Point<int>[] CurrentTetrominoPositions { get { return CurrentTetromino.Positions; } }
    public IBlock[] CurrentTetrominoBlocks { get { return CurrentTetromino.Blocks; } }
}
public struct PlacedBlock
{
    public int number;
    public ThreeSevenBlock type;
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