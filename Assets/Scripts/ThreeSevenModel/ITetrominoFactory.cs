using System.Collections;
using System;

namespace ThreeSeven.Model
{
    public interface ITetrominoFactory
    {
        Func<Tetromino> Create { get; }
    }
}
