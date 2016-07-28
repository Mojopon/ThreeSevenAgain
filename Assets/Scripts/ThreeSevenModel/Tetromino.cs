using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreeSeven.Model
{
    public struct TetrominoBlocks
    {
        public IBlock[] Blocks;
        public Point<int>[] Positions;
    }

    public class Tetromino
    {

        // Position of the Tetromino
        public Point<int> Position { get; set; }
        public TetrominoBlocks Blocks
        {
            get
            {
                return new TetrominoBlocks() { Blocks = _blocks.ToArray(), Positions = GetRelativePositionsFromPosition().ToArray() };
            }
        }
        public int Length { get { return _polyomino.Length; } }


        private List<Block> _blocks = new List<Block>();
        private Polyomino _polyomino;

        public Tetromino(Polyomino polyomino)
        {
            _polyomino = polyomino;

            for (int i = 0; i < _polyomino.Length; i++) _blocks.Add(Block.Create());
        }

        public void Turn(bool clockowise)
        {
            _polyomino.Turn(clockowise);
        }

        private IEnumerable<Point<int>> GetRelativePositionsFromPosition()
        {
            foreach(var point in _polyomino.Points)
            {
                yield return point.Add(Position);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var point in GetRelativePositionsFromPosition().ToArray())
            {
                builder.Append(string.Format("({0}, {1}) ", point.X, point.Y));
            }

            return builder.ToString();
        }
    }
}
