using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public class DisplayGameBoard : MonoBehaviour
{
    [SerializeField]
    private NumberBlock _BlockPrefab;

    private GameBoard _gameboard;
    private NumberBlock[,] _gameboardObjects;

    void Start()
    {
        _gameboard = new GameBoard(new Size<int>() { Width = 7, Height = 16 });

        _gameboardObjects = new NumberBlock[_gameboard.ActualSize.Width, _gameboard.ActualSize.Height];
        _gameboard.ActualCells.ForEach((point, cell) =>
        {
            _gameboardObjects[point.X, point.Y] = Instantiate(_BlockPrefab, point.ToVector3().InvertYAxis(), Quaternion.identity) as NumberBlock;
            _gameboardObjects[point.X, point.Y].SetNumber(cell.Block.GetNumber());
        });
    }
}
