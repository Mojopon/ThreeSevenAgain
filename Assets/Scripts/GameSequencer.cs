using UnityEngine;
using System.Collections;
using UniRx;

public class GameSequencer : Photon.MonoBehaviour
{
    [SerializeField]
    private ThreeSevenNetworkInputMessenger _ThreeSevenNetworkInputMessenger;

    [SerializeField]
    private PlayerGameBoardManager _PlayerGameBoardManager;
    [SerializeField]
    private GameObject _EnemyGameBoardManager;

    void Start()
    {
        _ThreeSevenNetworkInputMessenger.EventObservable
                                        .Subscribe(x => OnOtherPlayerJoins(x))
                                        .AddTo(gameObject);
    }

    public void OnJoinedRoom()
    {
        _PlayerGameBoardManager.StartGame();

        _ThreeSevenNetworkInputMessenger.SendGameBoardChange(ThreeSevenNetworkInputType.AddTetromino,new byte[] { 1, 2 });
    }

    public void OnOtherPlayerJoins(ThreeSevenNetworkInputEvent nEvent)
    {
        _EnemyGameBoardManager.SetActive(true);
    }
}
