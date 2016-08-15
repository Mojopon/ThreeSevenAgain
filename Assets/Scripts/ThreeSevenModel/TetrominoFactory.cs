using System;
using System.Collections;

namespace ThreeSeven.Model
{
    public class TetrominoFactory : ITetrominoFactory
    {
        public Func<Tetromino> Create { get; private set; }

        private Random _random = new Random();

        public TetrominoFactory()
        {
            Create = CreateNext;
        }

        public TetrominoFactory(Random random) : this()
        {
            _random = random;
        }

        private Tetromino CreateNext()
        {
            var tetromino = Tetromino.Create(Polyomino.Create(_random), () => Block.Create(_random));

            return tetromino;
        }
    }
}
