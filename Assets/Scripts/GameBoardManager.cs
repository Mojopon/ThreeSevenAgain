using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;
using System;

public class GameBoardManager : MonoBehaviour
{
    private GameBoard _gameboard;

    public GameBoard GameBoard
    {
        get
        {
            if(_gameboard == null)
            {
                InitializeGameBoard();
            }
            return _gameboard;
        }
        private set { _gameboard = value; }
    }

    public void StartGame()
    {
        StartCoroutine(SequenceStartGame());
    }

    private IEnumerator SequenceStartGame()
    {
        _gameboard.StartGame();

        yield break;
    }

    private void InitializeGameBoard ()
    {
        _gameboard = new GameBoard(GlobalSettings.GameBoardSize);

        _gameboard.StateObservable
                  .Where(x => x != GameBoardState.OnControlTetromino
                           && x != GameBoardState.BeforeDeleteBlocks)
                  .Delay(TimeSpan.FromMilliseconds(10f))
                  .Subscribe(x =>
                  {
                      _gameboard.GoNextState();
                  })
                  .AddTo(gameObject);

        _gameboard.StateObservable
                  .Where(x => x == GameBoardState.BeforeDeleteBlocks)
                  .Delay(TimeSpan.FromMilliseconds(200f))
                  .Subscribe(x =>
                  {
                      _gameboard.GoNextState();
                  })
                  .AddTo(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.A))
        {
            GameBoard.MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKeyDown(KeyCode.D))
        {
            GameBoard.MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Input.GetKeyDown(KeyCode.S))
        {
            GameBoard.MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) ||
                 Input.GetKeyDown(KeyCode.W))
        {
            GameBoard.Turn();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
        }
    }
}
