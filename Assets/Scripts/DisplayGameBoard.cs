using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;

public class DisplayGameBoard : MonoBehaviour
{
    [SerializeField]
    private GameBoardView _GameBoardView;

    [SerializeField]
    private NumberBlock _BlockPrefab;

    private GameBoard _gameboard;
    private NumberBlock[,] _gameboardObjects = null;

    void Awake()
    {
        _gameboard = new GameBoard(new Size<int>() { Width = 7, Height = 16 });
        _gameboard.StartGame();
    }

    void Start()
    {
        _GameBoardView.SetGameBoard(_gameboard);

        /*
        _gameboard.GameBoardObservable
                  .Subscribe(events => UpdateGameBoardObjects(events))
                  .AddTo(gameObject);
                  */

        _gameboard.AddNextTetromino();
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
            _gameboard.DropAllBlocks();
        }
    }

    /*
    void UpdateGameBoardObjects(GameBoardEvents events)
    {
        if (_gameboardObjects == null)
        {
            _gameboardObjects = new NumberBlock[_gameboard.Size.Width, _gameboard.Size.Height];
            events.Cells.ForEach((point, cell) => _gameboardObjects[point.X, point.Y] = Instantiate(_BlockPrefab, point.ToVector3().InvertYAxis(), Quaternion.identity) as NumberBlock);
        }

        events.Cells.ForEach((point, cell) => _gameboardObjects[point.X, point.Y].SetNumber(cell.Block.GetNumber()));

        if(events.TetrominoEvent.IsNotNull)
        {
            events.TetrominoEvent.CurrentTetromino.Foreach((point, block) => 
            {
                _gameboardObjects[point.X, point.Y].SetNumber(block.GetNumber());
            });
        }
        else
        {
            _gameboard.AddNextTetromino();
        }

        Debug.Log(_gameboard);
    }
    */
}
