using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;
using System.Linq;
using System;

public class GameBoardPresenter : MonoBehaviour
{
    [SerializeField]
    private SceneGameBoard _SceneGameBoard;

    [SerializeField]
    private GameBoardManager _GameBoardManager;

    void Start()
    {
        SubscribeGameBoard(_GameBoardManager.GameBoard);

        _GameBoardManager.StartGame();
    }

    private void SubscribeGameBoard(IGameBoardObservable gameBoard)
    {
        _SceneGameBoard.SetSize(GlobalSettings.GameBoardSize);

        gameBoard.GameBoardObservable
                  .Where(x => x != null)
                  .Select(x => x.TetrominoEvent)
                  .Where(x => x.NewTetrominoAdded)
                  .Select(x => x.CurrentTetromino.Blocks)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.AddTetromino(x.Select(block => block.Type).ToArray());
                  })
                  .AddTo(gameObject);

        gameBoard.GameBoardObservable
                  .Where(x => x != null && x.TetrominoEvent.HasEvent)
                  .Select(x => x.TetrominoEvent.CurrentTetromino)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.MoveTetromino(x.Positions);
                  })
                  .AddTo(gameObject);

        gameBoard.GameBoardObservable
                  .Where(x => x != null && x.TetrominoEvent.TetrominoIsPlaced)
                  .Subscribe(x =>
                  {
                      _SceneGameBoard.DestroyTetromino();
                  })
                  .AddTo(gameObject);

        gameBoard.GameBoardObservable
                  .Where(x => x != null && x.PlacedBlockEvent != null)
                  .Select(x => x.PlacedBlockEvent.placedBlocks)
                  .Subscribe(x =>
                  {
                      foreach (var placedBlock in x)
                          _SceneGameBoard.PlaceBlock(placedBlock.point, placedBlock.block.Type);
                  })
                  .AddTo(gameObject);

        gameBoard.GameBoardObservable
                  .Where(x => x != null && x.BlockMoveEvent != null && x.BlockMoveEvent.movements != null)
                  .Select(x => x.BlockMoveEvent.movements)
                  .Subscribe(x =>
                  {
                      foreach (var movement in x.OrderByDescending(y => y.source.Y))
                          _SceneGameBoard.MoveBlock(movement.source, movement.destination);
                  })
                  .AddTo(gameObject);

        gameBoard.GameBoardObservable
                  .Where(x => x != null && x.DeletedBlockEvent != null)
                  .Select(x => x.DeletedBlockEvent.deletedBlocks)
                  .Subscribe(x =>
                  {
                      foreach (var deletedBlock in x)
                          _SceneGameBoard.DeleteBlock(deletedBlock.point);
                  })
                  .AddTo(gameObject);
    }
}
