using UnityEngine;
using System.Collections;
using UniRx;
using ThreeSeven.Model;

public interface IGameBoardObservable
{
    IObservable<GameBoardEvents> GameBoardObservable { get; }
}
