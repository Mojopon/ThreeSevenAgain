using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;
using System.Collections.Generic;

public class SceneGameBoard : MonoBehaviour, ISceneGameBoard
{
    [SerializeField]
    private SceneBlock _BlockPrefab;

    private float blockScale = 1f;
    private Size<float> boardSize;
    private Vector3 center;

    private SceneTetromino _currentTetromino;
    private SceneBlock[,] _grid;


    public void SetSize(Size<int> size)
    {
        var width = size.Width * blockScale;
        var height = size.Height * blockScale;

        boardSize = new Size<float>() { Width = width, Height = height };
    }

    public void AddTetromino(ThreeSevenBlock[] blocks)
    {
        _currentTetromino = CreateSceneTetromino(blocks);
    }

    public void DestroyTetromino()
    {
        _currentTetromino.sceneBlocks.ForEach((count, block) =>
        {
            Destroy(block.gameObject);
        });

        _currentTetromino = null;
    }

    private SceneTetromino CreateSceneTetromino(ThreeSevenBlock[] blockTypes)
    {
        List<SceneBlock> sceneBlocks = new List<SceneBlock>();

        blockTypes.ForEach((type) => 
        {
            var sceneBlock = Instantiate(_BlockPrefab, Vector3.zero, Quaternion.identity) as SceneBlock;
            sceneBlock.SetNumber((int)type);
            sceneBlocks.Add(sceneBlock);
        });

        return new SceneTetromino(sceneBlocks.ToArray());
    }

    public void MoveTetromino(Point<int>[] points)
    {
        _currentTetromino.sceneBlocks.ForEach((count, block) =>
        {
            PlaceBlockToPoint(block, points[count]);
        });

        _currentTetromino.positions = points;
    }

    private void PlaceBlockToPoint(SceneBlock block, Point<int> point)
    {
        block.transform.localPosition = new Vector3(point.X, -point.Y);
    }

    public void PlaceTetromino()
    {
        _currentTetromino.positions.ForEach((count, point) =>
        {
            _grid[point.X, point.Y] = _currentTetromino.sceneBlocks[count];
        });

        _currentTetromino = null;
    }

    public void UpdateGrid(ThreeSevenBlock[,] cells)
    {

    }

    /*
    public void SetGameBoard(GameBoard gameboard)
    {
        boardSize = new Size<float>() { Width = gameboard.Size.Width, Height = gameboard.Size.Height };

        var width  = boardSize.Width * blockScale;
        var height = boardSize.Height * blockScale;

        var leftTop = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        var rightBottom = new Vector3(transform.position.x + width, transform.position.y - height, transform.position.z);
        var center = new Vector3(transform.position.x + (width / 2), transform.position.y - (height / 2), transform.position.z);

        _grid = new SceneBlock[gameboard.Size.Width, gameboard.Size.Height];

        _grid.ForEach((point, block) =>
        {
            var blockObject = Instantiate(_BlockPrefab, new Vector3(point.X, -point.Y), Quaternion.identity) as SceneBlock;
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

                var _tetrominoBlocks = new SceneBlock[_currentTetromino.Length];
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

    private SceneBlock InstantiateBlock(IBlock block)
    {
        SceneBlock numberBlockObject = null;

        numberBlockObject = Instantiate(_BlockPrefab, Vector3.zero, Quaternion.identity) as SceneBlock;
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
    */
}