using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public class CellBoardInitializer : MonoBehaviour {

	void Start ()
    {
        MaskedCellBoard maskedCellboard = new MaskedCellBoard(new Size<int> { Width = 7, Height = 16 });
    }
}
