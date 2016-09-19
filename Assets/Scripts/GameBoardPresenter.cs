using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;
using System.Linq;

public class GameBoardPresenter : MonoBehaviour
{
    [SerializeField]
    private SceneGameBoard _SceneGameBoard;

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

        
        _gameboard.GameBoardObservable
                  .Where (x => x != null)
                  .Select(x => x.TetrominoEvent)
                  .Where (x => x.State == CurrentTetrominoEventState.NewTetrominoAdded)
                  .Select(x => x.CurrentTetromino.Blocks)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.AddTetromino(x.Select(block => block.Type).ToArray());
                  });

        _gameboard.GameBoardObservable
                  .Where (x => x != null &&
                          x.TetrominoEvent.State == CurrentTetrominoEventState.ControlTetromino)
                  .Select(x => x.TetrominoEvent.CurrentTetromino)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.MoveTetromino(x.Positions);
                  });



        _gameboard.StartGame();
    }

    void Start()
    {
        _gameboard.AddNextTetromino();
    }

    private void SetGameBoardEventToSceneGameBoard(GameBoardEvents gameBoardEvents)
    {
        
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
