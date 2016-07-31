using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public class CellBoardInitializer : MonoBehaviour {

    void Start()
    {
        MaskedCellBoard maskedCellboard = new MaskedCellBoard(new Size<int> { Width = 7, Height = 16 });

        Tetromino tetromino = new Tetromino(Polyomino.Create(PolyominoIndex.I), () => Block.Create());

        Debug.Log(tetromino);

        tetromino.Position = new Point<int>() { X = 1, Y = 2 };

        Debug.Log(tetromino);

    }
}

