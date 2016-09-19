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
            sceneBlock.SetNumber((int)type);
            sceneBlocks.Add(sceneBlock);
        });

        return new SceneTetromino(sceneBlocks.ToArray());
    }

    public void DestroyTetromino()
    {
        _currentTetromino.sceneBlocks.ForEach((count, block) =>
        {
            Destroy(block.gameObject);
        });

        _currentTetromino = null;
    }

    public void MoveTetromino(Point<int>[] points)
    {
        _currentTetromino.sceneBlocks.ForEach((count, block) =>
        {
            PlaceBlockToPoint(points[count], block);
        });

        _currentTetromino.positions = points;
    }

    private void PlaceBlockToPoint(Point<int> point, SceneBlock block)
    {
        block.transform.localPosition = new Vector3(point.X, -point.Y);
    }

    public void PlaceBlock(Point<int> point, ThreeSevenBlock type)
    {
        var sceneBlock = Instantiate(_BlockPrefab, Vector3.zero, Quaternion.identity) as SceneBlock;
        sceneBlock.SetNumber((int)type);
        sceneBlock.transform.localPosition = new Vector3(point.X, -point.Y);
        _grid[point.X, point.Y] = sceneBlock;
    }

    public void MoveBlock(Point<int> source, Point<int> destination)
    {
        if (_grid[source.X, source.Y] == null) return;
        _grid[source.X, source.Y].MoveTo(new Vector3(destination.X, -destination.Y));

        _grid[destination.X, destination.Y] = _grid[source.X, source.Y];
        _grid[source.X, source.Y] = null;
    }

    public void UpdateGrid(ThreeSevenBlock[,] cells)
    {

    }
}