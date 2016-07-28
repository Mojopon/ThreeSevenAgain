using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace ThreeSeven.Model
{

    /*
    public class Tetromino
    {
        static Random random = new Random();
        
        public bool[,] shape;

        public PolyominoIndex Index { get; set; }

        private static Dictionary<PolyominoIndex, bool[,]> _table = null;
        private Dictionary<PolyominoIndex, bool[,]> Table
        {
            get
            {
                if (_table == null)
                {
                    InitializeTable();
                }
                return _table;
            }
        }

        public Point<int> Position { get; set; }

        public Size<int> Size
        {
            get { return shape.Size(); }
        }

        void InitializeTable()
        {
            _table = new Dictionary<PolyominoIndex, bool[,]>
            {
                { PolyominoIndex.I, new bool[,]
                {{ false, false, false, false },
                 { true , true , true , true  },
                 { false, false, false, false },
                 { false, false, false, false }}
                },
                { PolyominoIndex.N, new bool[,]
                {{ false, false, true , false },
                 { false, true , true , false },
                 { false, true , false, false },
                 { false, false, false, false }}
                },
                { PolyominoIndex.Z, new bool[,]
                {{ false, true , false},
                 { false, true , true },
                 { false, false, true }}
                },
            };
        }

        public Tetromino()
        {
            Index = GetRandomPolyominoIndex();
            SelectShape();
        }

        public Tetromino(PolyominoIndex index)
        {
            Index = index;
            SelectShape();
        }

        void SelectShape()
        {
            shape = (bool[,])Table[Index].Turn().Clone();
        }

        public int CountSpace()
        {
            return shape.ToSequence().Where(x => x).Count();
        }

        PolyominoIndex GetRandomPolyominoIndex()
        {
            return (PolyominoIndex)(random.Next(Table.Count) + 1);
        }
    }
    */
}
