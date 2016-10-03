using UnityEngine;
using System.Collections;
using Photon;
using UniRx;

public enum ThreeSevenNetworkInputType
{
    None = 0,
    Entry,
    AddTetromino,
    MoveTetromino,
}

public struct ThreeSevenNetworkInputEvent
{
    public int id;
    public ThreeSevenNetworkInputType type;
    public byte[] data;
}

public class ThreeSevenNetwork : Photon.MonoBehaviour
{
    public IObservable<ThreeSevenNetworkInputEvent> EventObservable { get { return _eventStream.AsObservable(); } }
    public ISubject<ThreeSevenNetworkInputEvent> _eventStream = new Subject<ThreeSevenNetworkInputEvent>();

    [PunRPC]
    public void OnGameBoardChange(int id, byte inputType, byte[] data, PhotonMessageInfo info)
    {
        _eventStream.OnNext(new ThreeSevenNetworkInputEvent()
        {
            id = id,
            type = (ThreeSevenNetworkInputType)inputType,
            data = data,
        });
    }

    public void SendGameBoardChange(int id, ThreeSevenNetworkInputType inputType,  object data)
    {
        var byteInputType = (byte)inputType;

        byte[] byteData = (byte[])data;

        photonView.RPC("OnGameBoardChange", PhotonTargets.OthersBuffered, id, byteInputType, byteData);
    }
}
