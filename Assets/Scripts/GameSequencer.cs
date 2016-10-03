using UnityEngine;
using System.Collections;
using UniRx;

public class GameSequencer : Photon.MonoBehaviour
{
    [SerializeField]
    private ThreeSevenNetwork _ThreeSevenNetwork;

    [SerializeField]
    private PlayerGameBoardManager _PlayerGameBoardManager;
    [SerializeField]
    private GameObject _EnemyGameBoardManager;

    void Start()
    {
        _PlayerGameBoardManager.GameBoardObservable
                               .Where(x => x != null)
                               .Subscribe(x => SubscribeOnGameBoard(x))
                               .AddTo(gameObject);

        _ThreeSevenNetwork.EventObservable
                          .Subscribe(x => OnInputReceived(x))
                          .AddTo(gameObject);
    }


    private void SubscribeOnGameBoard(IGameBoardObservable gameBoard)
    {
        // On New Tetromino Added
        gameBoard.GameBoardEventsObservable
                  .Where(x => x != null)
                  .Select(x => x.TetrominoEvent)
                  .Where(x => x.NewTetrominoAdded)
                  .Select(x => x.CurrentTetromino.Blocks)
                  .Subscribe(x =>
                  {
                      _ThreeSevenNetwork.SendGameBoardChange
                      (PhotonNetwork.player.ID,                                     
                       ThreeSevenNetworkInputType.AddTetromino,
                       new byte[] { 1, 2 });

                      //_SceneGameBoard.AddTetromino(x.Select(block => block.Type).ToArray());
                  })
                  .AddTo(gameObject);

        // On Tetromino Moved
        gameBoard.GameBoardEventsObservable
                  .Where(x => x != null && x.TetrominoEvent.HasEvent)
                  .Select(x => x.TetrominoEvent.CurrentTetromino)
                  .Subscribe(x =>
                  {
                      _ThreeSevenNetwork.SendGameBoardChange
                      (PhotonNetwork.player.ID,
                       ThreeSevenNetworkInputType.MoveTetrominoToDirection,
                       new byte[] { 1, 2 });
                      //_SceneGameBoard.MoveTetromino(x.Positions);
                  })
                  .AddTo(gameObject);

        // On Tetromino Placed, Destroy Current Tetromino
        gameBoard.GameBoardEventsObservable
                  .Where(x => x != null && x.TetrominoEvent.TetrominoIsPlaced)
                  .Subscribe(x =>
                  {
                      _ThreeSevenNetwork.SendGameBoardChange
                      (PhotonNetwork.player.ID,
                       ThreeSevenNetworkInputType.DestroyTetromino,
                       new byte[] { 1, 2 });
                      //_SceneGameBoard.DestroyTetromino();
                  })
                  .AddTo(gameObject);

        // On Tetromino Placed, Add New Blocks to the SceneGameBoard instead of the Destroyed Tetromino
        gameBoard.GameBoardEventsObservable
                  .Where(x => x != null && x.PlacedBlockEvent != null)
                  .Select(x => x.PlacedBlockEvent.placedBlocks)
                  .Subscribe(x =>
                  {
                      _ThreeSevenNetwork.SendGameBoardChange
                      (PhotonNetwork.player.ID,
                       ThreeSevenNetworkInputType.PlaceBlocks,
                       new byte[] { 1, 2 });
                      //foreach (var placedBlock in x)
                      //    _SceneGameBoard.PlaceBlock(placedBlock.point, placedBlock.type);
                  })
                  .AddTo(gameObject);

        // On Block Drop
        gameBoard.GameBoardEventsObservable
                  .Where(x => x != null && x.BlockMoveEvent != null && x.BlockMoveEvent.movements != null)
                  .Select(x => x.BlockMoveEvent.movements)
                  .Subscribe(x =>
                  {
                      _ThreeSevenNetwork.SendGameBoardChange
                      (PhotonNetwork.player.ID,
                       ThreeSevenNetworkInputType.DropBlocks,
                       new byte[] { 1, 2 });
                      //foreach (var movement in x.OrderByDescending(y => y.source.Y))
                      //    _SceneGameBoard.MoveBlock(movement.source, movement.destination);
                  })
                  .AddTo(gameObject);

        // On Block Delete
        gameBoard.GameBoardEventsObservable
                  .Where(x => x != null && x.DeletedBlockEvent != null)
                  .Select(x => x.DeletedBlockEvent.deletedBlocks)
                  .Subscribe(x =>
                  {
                      _ThreeSevenNetwork.SendGameBoardChange
                      (PhotonNetwork.player.ID,
                       ThreeSevenNetworkInputType.DeleteBlocks,
                       new byte[] { 1, 2 });
                      //foreach (var deletedBlock in x)
                      //    _SceneGameBoard.DeleteBlock(deletedBlock.point);
                  })
                  .AddTo(gameObject);
    }

    public void OnJoinedRoom()
    {
        _ThreeSevenNetwork.SendGameBoardChange(PhotonNetwork.player.ID, ThreeSevenNetworkInputType.Entry, new byte[] { 1, 2 });

        _PlayerGameBoardManager.StartGame();
    }

    private void OnInputReceived(ThreeSevenNetworkInputEvent nEvent)
    {
        DebugMessageUI.Instance.SetMessage(nEvent.type.ToString());

        switch (nEvent.type)
        {
            case ThreeSevenNetworkInputType.Entry:
                OnPlayerEntryReceived(nEvent);
                break;
        }
    }

    private void OnPlayerEntryReceived(ThreeSevenNetworkInputEvent nEvent)
    {
        _EnemyGameBoardManager.SetActive(true);
    }
}
