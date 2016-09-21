using System.Collections;

namespace ThreeSeven.Model
{
    public struct Cell
    {
        private Block _block;

        public IBlock Block
        {
            get
            {
                if (_block == null)
                {
                    return new NoBlock();
                }

                return _block;
            }
        }

        public void Set(IBlock block)
        {
            _block = (Block)block;
        }

        public void Set(int number)
        {
            _block = ThreeSeven.Model.Block.Create(number);
        }

        public bool IsNull { get { return (_block == null || _block.Type == ThreeSevenBlock.None); } }

        public void Clear()
        {
            _block = null;
        }
    }

    public static class CellsExtension
    {
        public static int[,] CellsToNumberGrid(this Cell[,] @this)
        {
            var array = new int[@this.GetLength(0), @this.GetLength(1)];

            @this.ForEach((point, cell) =>
            {
                array[point.X, point.Y] = cell.Block.GetNumber();
            });

            return array;
        }

        public static bool[,] CellsToBoolGrid(this Cell[,] @this)
        {
            var array = new bool[@this.GetLength(0), @this.GetLength(1)];

            @this.ForEach((point, cell) =>
            {
                array[point.X, point.Y] = cell.IsNull;
            });

            return array;
        }
    }
    
}
