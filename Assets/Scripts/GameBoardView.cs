using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;
using System.Collections.Generic;

public class GameBoardView : MonoBehaviour
{
    [SerializeField]
    private NumberBlock _BlockPrefab;

    public float blockScale = 1f;
    private Size<float> boardSize;
    private Vector3 center;

    private Dictionary<Tetromino, NumberBlock[]> _tetrominoObjectsDictionary;
    private NumberBlock[,] _grid;


    private Tetromino _currentTetromino;

    private void Awake()
    {
        _tetrominoObjectsDictionary = new Dictionary<Tetromino, NumberBlock[]>();
    }

    public void SetGameBoard(GameBoard gameboard)
    {
        boardSize = new Size<float>() { Width = gameboard.Size.Width, Height = gameboard.Size.Height };

        var width  = boardSize.Width * blockScale;
        var height = boardSize.Height * blockScale;

        var leftTop = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        var rightBottom = new Vector3(transform.position.x + width, transform.position.y - height, transform.position.z);
        var center = new Vector3(transform.position.x + (width / 2), transform.position.y - (height / 2), transform.position.z);

        _grid = new NumberBlock[gameboard.Size.Width, gameboard.Size.Height];

        _grid.ForEach((point, block) =>
        {
            var blockObject = Instantiate(_BlockPrefab, new Vector3(point.X, -point.Y), Quaternion.identity) as NumberBlock;
            _grid[point.X, point.Y] = blockObject;

            blockObject.gameObject.SetActive(false);
        });

        gameboard.GameBoardObservable
                 .Subscribe(events => UpdateGameboardGrid(events))
                 .AddTo(gameObject);
    }

    private void UpdateGameboardGrid(GameBoardEvents gameBoardEvents)
    {
        if (gameBoardEvents.TetrominoEvent.HasEvent)
        {
            if (_currentTetromino != gameBoardEvents.TetrominoEvent.CurrentTetromino)
            {
                _currentTetromino = gameBoardEvents.TetrominoEvent.CurrentTetromino;

                var _tetrominoBlocks = new NumberBlock[_currentTetromino.Length];
                _currentTetromino.Foreach((count, point, block) =>
                {
                    _tetrominoBlocks[count] = InstantiateBlock(block);
                });

                _tetrominoObjectsDictionary.Add(_currentTetromino, _tetrominoBlocks);
            }

            UpdateCurrentTetrominoObjectsPosition();
        }
        else if(!gameBoardEvents.TetrominoEvent.HasEvent && _currentTetromino != null)
        {
            DestroyCurrentTetrominoObjects();
        }

        UpdateGrid(gameBoardEvents.Cells);
    }

    private NumberBlock InstantiateBlock(IBlock block)
    {
        NumberBlock numberBlockObject = null;

        numberBlockObject = Instantiate(_BlockPrefab, Vector3.zero, Quaternion.identity) as NumberBlock;
        numberBlockObject.SetNumber(block.GetNumber());
        return numberBlockObject;
    }

    private void UpdateCurrentTetrominoObjectsPosition()
    {
        var blocks = _tetrominoObjectsDictionary[_currentTetromino];

        _currentTetromino.Foreach((count, point, block) =>
        {
            blocks[count].transform.position = new Vector3(point.X, -point.Y);
        });
    }

    private void DestroyCurrentTetrominoObjects()
    {
        var blocks = _tetrominoObjectsDictionary[_currentTetromino];

        _currentTetromino.Foreach((count, point, block) =>
        {
            Destroy(blocks[count].gameObject);
        });

        _tetrominoObjectsDictionary.Remove(_currentTetromino);
        _currentTetromino = null;
    }

    private void UpdateGrid(Cell[,] cells)
    {
        cells.ForEach((point, cell) =>
        {
            if (!cell.IsNull)
            {
                _grid[point.X, point.Y].gameObject.SetActive(true);
                _grid[point.X, point.Y].SetNumber(cell.Block.GetNumber());
            }
            else
            {
                _grid[point.X, point.Y].gameObject.SetActive(false);
            }
        });
    }

    public void OnTetrominoMove()
    {

    }
}