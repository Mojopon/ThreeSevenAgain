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
        _SceneGameBoard.SetSize(_gameboard.Size);

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
                  .Where (x => x.NewTetrominoAdded)
                  .Select(x => x.CurrentTetromino.Blocks)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.AddTetromino(x.Select(block => block.Type).ToArray());
                  });

        _gameboard.GameBoardObservable
                  .Where (x => x != null && x.TetrominoEvent.HasEvent)
                  .Select(x => x.TetrominoEvent.CurrentTetromino)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.MoveTetromino(x.Positions);
                  });

        _gameboard.GameBoardObservable
                  .Where(x => x != null && x.TetrominoEvent.TetrominoIsPlaced)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.DestroyTetromino();
                  });

        _gameboard.GameBoardObservable
                  .Where(x => x != null && x.PlacedBlockEvent != null)
                  .Select(x => x.PlacedBlockEvent.placedBlocks)
                  .Subscribe(x =>
                  {
                      foreach (var placedBlock in x)
                          _SceneGameBoard.PlaceBlock(placedBlock.point, placedBlock.block.Type);
                  });

        _gameboard.GameBoardObservable
                  .Where(x => x != null && x.BlockMoveEvent != null && x.BlockMoveEvent.movements != null)
                  .Select(x => x.BlockMoveEvent.movements)
                  .Subscribe(x =>
                  {
                      foreach (var movement in x.OrderByDescending(y => y.source.Y))
                          _SceneGameBoard.MoveBlock(movement.source, movement.destination);
                  });

        _gameboard.StartGame();
    }

    void Start()
    {
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
