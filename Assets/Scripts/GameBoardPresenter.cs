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
                  .Where(x => x != GameBoardState.OnControlTetromino)
                  .Skip(1)
                  .Subscribe(x =>
                  {
                      _gameboard.GoNextState();
                  })
                  .AddTo(gameObject);


        _gameboard.GameBoardObservable
                  .Where(x => x != null)
                  .Select(x => x.TetrominoEvent)
                  .Where(x => x.NewTetrominoAdded)
                  .Select(x => x.CurrentTetromino.Blocks)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.AddTetromino(x.Select(block => block.Type).ToArray());
                  });

        _gameboard.GameBoardObservable
                  .Where(x => x != null && x.TetrominoEvent.HasEvent)
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

        _gameboard.GameBoardObservable
                  .Where(x => x != null && x.DeletedBlockEvent != null)
                  .Select(x => x.DeletedBlockEvent.deletedBlocks)
                  .Subscribe(x =>
                  {
                      foreach (var deletedBlock in x)
                          _SceneGameBoard.DeleteBlock(deletedBlock.point);
                  });

        _gameboard.StartGame();
    }

    void Start()
    {
        _gameboard.GoNextState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.A))
        {
            _gameboard.MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKeyDown(KeyCode.D))
        {
            _gameboard.MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Input.GetKeyDown(KeyCode.S))
        {
            _gameboard.MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)||
                 Input.GetKeyDown(KeyCode.W))
        {
            _gameboard.Turn();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            _gameboard.DropAllBlocks();
        }
    }
}
