using System.Collections;
using UnityEngine;

namespace ThreeSeven.Model
{
    public class CellBoard
    {
        public Size<int> Size { get; protected set; }
        public Point<int> Center { get { return new Point<int> { X = Size.Width / 2, Y = Size.Height / 2 }; } }

        public Cell[,] Cells { get; protected set; }
        public Cell[,] CellsClone
        {
            get
            {
                var cellsClone = new Cell[Size.Width, Size.Height];
                Cells.ForEach((point, cell) => cellsClone.Set(point, cell));
                return cellsClone;
            }
            set
            {
                value.ForEach((point, cell) => Cells.Set(point, cell));
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
}
