using System.Collections;
using UnityEngine;

namespace ThreeSeven.Model
{
    public class CellBoard
    {
        public Size<int> Size { get; protected set; }
        public virtual Size<int> ActualSize { get { return Size; } }

        public Cell[,] Cells { get; protected set; }
        public virtual Cell[,] ActualCells { get { return Cells; } }

        public Block[,] CellsClone
        {
            get
            {
                var cellsClone = new Block[Size.Width, Size.Height];
                Cells.ForEach((point, cell) => cellsClone.Set(point, cell.Block));
                return cellsClone;
            }
            set
            {
                Cells.ForEach((point, cell) => cell.Block = value.Get(point));
            }
        }

        public CellBoard(Size<int> size)
        {
            Size = size;
            Cells = new Cell[Size.Width, Size.Height];
            Cells.ForEach((point, cell) => Cells.Set(point, new Cell()));
        }

        public virtual void Clear()
        {
            Cells.ForEach((point, cell) => cell.Clear());
        }
    }

    public class MaskedCellBoard : CellBoard
    {
        private readonly int topMask = 2;

        public int TopMask { get; set; }

        public override Size<int> ActualSize { get { return new Size<int> { Width = Size.Width, Height = Size.Height - TopMask }; } }

        public override Cell[,] ActualCells
        {
            get
            {
                var actualCells = new Cell[ActualSize.Width, ActualSize.Height];
                actualCells.AllPoints()
                           .ForEach(point => actualCells.Set(point, Cells.Get(point.Add(new Size<int> { Width = 0, Height = TopMask }))));
                return actualCells;
            }
        }

        public MaskedCellBoard(Size<int> size) : base(size)
        {
            TopMask = topMask;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
