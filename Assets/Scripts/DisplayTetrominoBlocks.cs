using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using System.Collections.Generic;

public class DisplayTetrominoBlocks : MonoBehaviour
{
    [SerializeField]
    private NumberBlock _NumberBlockPrefab;

    private List<NumberBlock> _spawnedBlocks = new List<NumberBlock>();

    private Tetromino _tetromino;

    private void Start()
    {
        _tetromino = new Tetromino(Polyomino.Create(PolyominoIndex.N), () => Block.Create());

        _tetromino.Foreach((point, block) => _spawnedBlocks.Add(Instantiate(_NumberBlockPrefab)));

        PlaceBlocks();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Turn();
        }
    }

    private void PlaceBlocks()
    {
        _tetromino.Foreach((index, point, block) =>
        {
            var blockObject = _spawnedBlocks[index++];
            blockObject.SetNumber(block.GetNumber());
            blockObject.transform.position = point.ToVector3().InvertYAxis();
        });
    }

    private void Turn()
    {
        _tetromino.Turn(true);
        PlaceBlocks();
    }
}
