using UnityEngine;
using System.Collections;
using Photon;
using UniRx;

public enum ThreeSevenNetworkInputType
{
    None = 0,
    AddTetromino,
    MoveTetromino,
}

public struct ThreeSevenNetworkInputEvent
{
    public ThreeSevenNetworkInputType type;
    public byte[] data;
}

public class ThreeSevenNetworkInputMessenger : Photon.MonoBehaviour
{
    public IObservable<ThreeSevenNetworkInputEvent> EventObservable { get { return _eventStream.AsObservable(); } }
    public ISubject<ThreeSevenNetworkInputEvent> _eventStream = new Subject<ThreeSevenNetworkInputEvent>();

    [PunRPC]
    public void OnGameBoardChange(int inputType, byte[] data, PhotonMessageInfo info)
    {
        var type = (ThreeSevenNetworkInputType)inputType;

        _eventStream.OnNext(new ThreeSevenNetworkInputEvent()
        {
            type = type,
            data = data,
        });

        DebugMessageUI.Instance.SetMessage("On GameBoard Change was Called");
    }

    public void SendGameBoardChange(ThreeSevenNetworkInputType inputType, object data)
    {
        var typeNumber = (int)inputType;

        byte[] byteData = (byte[])data;

        photonView.RPC("OnGameBoardChange", PhotonTargets.OthersBuffered, typeNumber, byteData);
    }
}
