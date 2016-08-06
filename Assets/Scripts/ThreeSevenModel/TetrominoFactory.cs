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

        {
            _random = random;
        }

        private Tetromino CreateNext()
        {

            return tetromino;
        }
    }
}
