using System;
using System.Collections;
using System.Collections.Generic;

namespace ThreeSeven.Model
{
    public class TetrominoFactory : ITetrominoFactory
    {
        public Func<Tetromino> Create { get; private set; }

        private Random _random = new Random();
        private Func<Block> _blockFactory = null;
        private Func<Polyomino> _polyominoFactory = null;

        public TetrominoFactory()
        {
            _blockFactory = DefaultBlockFactory();
            _polyominoFactory = () => Polyomino.Create(new Random(0));

            Create = CreateNext;
        }

        public TetrominoFactory(Random random) : this()
        {
            _blockFactory = () =>
            {
                return Block.Create(random);
            };

            _polyominoFactory = () => Polyomino.Create(random);
        }

        public TetrominoFactory(Func<Polyomino> polyominoFactory) : this()
        {
            _polyominoFactory = polyominoFactory;
        }

        private Tetromino CreateNext()
        {
            var tetromino = Tetromino.Create(_polyominoFactory(), _blockFactory);

            return tetromino;
        }

        private Func<Block> DefaultBlockFactory()
        {
            List<Block> blocks = null;

            return () =>
            {
                if (blocks == null || 0 >= blocks.Count)
                {
                    blocks = new List<Block>()
                    {
                        Block.Create(ThreeSevenBlock.One),
                        Block.Create(ThreeSevenBlock.Two),
                        Block.Create(ThreeSevenBlock.Three),
                        Block.Create(ThreeSevenBlock.Four),
                        Block.Create(ThreeSevenBlock.Five),
                        Block.Create(ThreeSevenBlock.Six),
                        Block.Create(ThreeSevenBlock.Seven),
                    };
                }

                var block = blocks[0];
                blocks.RemoveAt(0);
                return block;
            };
        }
    }
}
