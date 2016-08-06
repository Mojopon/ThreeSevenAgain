using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;

public class DisplayGameBoard : MonoBehaviour
{
    [SerializeField]
    private NumberBlock _BlockPrefab;

    private GameBoard _gameboard;
    private NumberBlock[,] _gameboardObjects = null;

    void Start()
    {
        _gameboard = new GameBoard(new Size<int>() { Width = 7, Height = 16 });
        _gameboard.SetTetrominoFactory(new TetrominoFactory(new System.Random(893)));

        _gameboard.GameBoardCellsObservable
                  .Subscribe(cells => UpdateGameBoardObjects(cells))
                  .AddTo(gameObject);

        _gameboard.Start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _gameboard.MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _gameboard.MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _gameboard.MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _gameboard.Turn();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            _gameboard.Place();
        }
    }

    void UpdateGameBoardObjects(Cell[,] cells)
    {
        if (_gameboardObjects == null)
        {
            _gameboardObjects = new NumberBlock[_gameboard.ActualSize.Width, _gameboard.ActualSize.Height];
            _gameboard.ActualCells.ForEach((point, cell) => _gameboardObjects[point.X, point.Y] = Instantiate(_BlockPrefab, point.ToVector3().InvertYAxis(), Quaternion.identity) as NumberBlock);
        }

        _gameboard.ActualCells.ForEach((point, cell) => _gameboardObjects[point.X, point.Y].SetNumber(cell.Block.GetNumber()));
    }
}
