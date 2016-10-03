using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using UniRx;
using System.Collections.Generic;

public class SceneGameBoard : MonoBehaviour, ISceneGameBoard
{
    [SerializeField]
    private SceneBlock _BlockPrefab;

    [SerializeField]
    private float blockScale = 1f;
    [SerializeField]
    private Size<float> boardSize;

    private SceneTetromino _currentTetromino;
    private SceneBlock[,] _grid;

    private Transform _blockHolder;

    void Awake()
    {
        _blockHolder = new GameObject("Block Holder").transform;
        _blockHolder.transform.SetParent(gameObject.transform);
    }

    public void SetSize(Size<int> size)
    {
        var width = size.Width * blockScale;
        var height = size.Height * blockScale;

        boardSize = new Size<float>() { Width = width, Height = height };
        _grid = new SceneBlock[size.Width, size.Height];
    }

    public void AddTetromino(ThreeSevenBlock[] blocks)
    {
        _currentTetromino = CreateSceneTetromino(blocks);
    }

    private SceneTetromino CreateSceneTetromino(ThreeSevenBlock[] blockTypes)
    {
        List<SceneBlock> sceneBlocks = new List<SceneBlock>();

        blockTypes.ForEach((type) => 
        {
            var sceneBlock = Instantiate(_BlockPrefab, Vector3.zero, Quaternion.identity) as SceneBlock;
            sceneBlock.transform.SetParent(_blockHolder);
            sceneBlock.SetNumber((int)type);
            sceneBlocks.Add(sceneBlock);
        });

        return new SceneTetromino(sceneBlocks.ToArray());
    }

    public void MoveTetromino(Point<int>[] points)
    {
        _currentTetromino.sceneBlocks.ForEach((count, block) =>
        {
            PlaceBlockToPoint(points[count], block);
        });

        _currentTetromino.positions = points;
    }

    public void MoveTetromino(Direction direction)
    {
        var currentPositions = _currentTetromino.positions;
        currentPositions.ForEach((position) => position.Add(direction.ToPoint()));

        _currentTetromino.sceneBlocks.ForEach((count, block) =>
        {
            PlaceBlockToPoint(currentPositions[count], block);
        });

        _currentTetromino.positions = currentPositions;
    }

    public void DestroyTetromino()
    {
        _currentTetromino.sceneBlocks.ForEach((count, block) =>
        {
            Destroy(block.gameObject);
        });

        _currentTetromino = null;
    }

    private void PlaceBlockToPoint(Point<int> point, SceneBlock block)
    {
        block.transform.localPosition = BlockPositionFromPoint(point);
    }

    public void PlaceBlock(Point<int> point, ThreeSevenBlock type)
    {
        var sceneBlock = Instantiate(_BlockPrefab, Vector3.zero, Quaternion.identity) as SceneBlock;
        sceneBlock.transform.SetParent(_blockHolder);
        sceneBlock.SetNumber((int)type);
        sceneBlock.transform.localPosition = BlockPositionFromPoint(point);
        _grid[point.X, point.Y] = sceneBlock;
    }

    public void MoveBlock(Point<int> source, Point<int> destination)
    {
        if (_grid[source.X, source.Y] == null) return;
        _grid[source.X, source.Y].MoveTo(BlockPositionFromPoint(destination));

        _grid[destination.X, destination.Y] = _grid[source.X, source.Y];
        _grid[source.X, source.Y] = null;
    }

    public void DeleteBlock(Point<int> point)
    {
        if (_grid[point.X, point.Y] == null) return;

        _grid[point.X, point.Y].Delete();
        _grid[point.X, point.Y] = null;
    }

    private Vector3 BlockPositionFromPoint(Point<int> point)
    {
        var position = point.ToVector3().InvertYAxis();
        position += transform.position;

        return position;
    }

    void OnDrawGizmos()
    {
        var offset = new Vector3(blockScale / 2, blockScale / 2);
        var topLeft = transform.position - offset;
        topLeft = new Vector3(topLeft.x, -topLeft.y);
        var bottomRight = transform.position + new Vector3(boardSize.Width, boardSize.Height, 0) - offset;
        bottomRight = new Vector3(bottomRight.x, -bottomRight.y);


        var topRight =   new Vector3(bottomRight.x, topLeft.y);
        var bottomLeft = new Vector3(topLeft.x, bottomRight.y);


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(topLeft,    topRight);
        Gizmos.DrawLine(topLeft,    bottomLeft);
        Gizmos.DrawLine(topRight,   bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
    }
}