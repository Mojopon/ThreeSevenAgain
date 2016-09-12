using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;

public class GameBoardPresenter : MonoBehaviour
{
    [SerializeField]
    private GameBoardView _GameBoardView;

    private GameBoard _gameboard;

    void Awake()
    {
        _gameboard = new GameBoard(new Size<int>() { Width = 7, Height = 16 });

        _gameboard.StateObservable
                  .Where(x => x == GameBoardState.BeforeAddTetromino ||
                              x == GameBoardState.BeforeDropBlocks)
                  .Subscribe(x =>
                  {
                      _gameboard.GoNextState();
                  })
                  .AddTo(gameObject);

        _gameboard.StartGame();
    }

    void Start()
    {
        _GameBoardView.SetGameBoard(_gameboard);

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
}
