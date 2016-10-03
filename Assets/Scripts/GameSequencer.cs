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
        _ThreeSevenNetwork.EventObservable
                          .Where(x => x.type == ThreeSevenNetworkInputType.Entry)
                          .Subscribe(x => OnInputReceived(x))
                          .AddTo(gameObject);
    }

    public void OnJoinedRoom()
    {
        _ThreeSevenNetwork.SendGameBoardChange(PhotonNetwork.player.ID, ThreeSevenNetworkInputType.Entry, new byte[] { 1, 2 });

        _PlayerGameBoardManager.StartGame();
    }

    private void OnInputReceived(ThreeSevenNetworkInputEvent nEvent)
    {
        switch(nEvent.type)
        {
            case ThreeSevenNetworkInputType.Entry:
                OnPlayerEntry(nEvent);
                break;
        }
    }

    private void OnPlayerEntry(ThreeSevenNetworkInputEvent nEvent)
    {
        _EnemyGameBoardManager.SetActive(true);
    }
}
