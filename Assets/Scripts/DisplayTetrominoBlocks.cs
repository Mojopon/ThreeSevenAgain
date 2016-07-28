using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using System.Collections.Generic;

public class DisplayTetrominoBlocks : MonoBehaviour
{
    [SerializeField]
    private NumberBlock _NumberBlock;

    private List<NumberBlock> _spawnedBlocks = new List<NumberBlock>();

    private Tetromino _tetromino;

    private void Start()
    {
        _tetromino = new Tetromino(Polyomino.Create( PolyominoIndex.N));

        for(int i = 0; i < _tetromino.Length; i++)
        {
            _spawnedBlocks.Add(Instantiate(_NumberBlock));
        }

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
        var tetrominoBlocks = _tetromino.Blocks;
        for(int i = 0; i < _spawnedBlocks.Count; i++)
        {
            var block = _spawnedBlocks[i];
            block.transform.position = tetrominoBlocks.Positions[i].ToVector3().InvertYAxis();
            block.SetNumber(tetrominoBlocks.Blocks[i].GetNumber());
        }
    }

    private void Turn()
    {
        _tetromino.Turn(true);
        PlaceBlocks();
    }
}
