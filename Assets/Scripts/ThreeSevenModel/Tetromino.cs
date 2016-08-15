using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace ThreeSeven.Model
{

    public class Tetromino
    {
        // Position of the Tetromino
        public Point<int>   Position  { get; set; }

        public IBlock[]     Blocks    { get { return _blocks.ToArray(); } }
        public Point<int>[] Positions { get { return GetRelativePositionsFromPosition().ToArray(); } }

        public int          Length    { get { return _polyomino.Length; } }
        public Size<int>    Size      { get { return _polyomino.Size; } }

        private List<Block> _blocks = new List<Block>();
        private Polyomino _polyomino;

        public static Tetromino Create(Polyomino polyomino, Func<Block> blockFactory)
        {
            return new Tetromino(polyomino, blockFactory);
        }

        private Tetromino(Polyomino polyomino, Func<Block> blockFactory)
        {
            _polyomino = polyomino;

            for (int i = 0; i < _polyomino.Length; i++) _blocks.Add(blockFactory());
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

    public static class TetrominoExtention
    {
        public static void Foreach(this Tetromino @this, Action<Point<int>, IBlock> action)
        {
            if (@this == null) return;

            for(int i = 0; i < @this.Length; i++)
            {
                action(@this.Positions[i], @this.Blocks[i]);
            }
        }

        public static void Foreach(this Tetromino @this, Action<int, Point<int>, IBlock> action)
        {
            if (@this == null) return;

            var index = 0;
            for (int i = 0; i < @this.Length; i++)
            {
                action(index, @this.Positions[i], @this.Blocks[i]);
                index++;
            }
        }
    }
}
