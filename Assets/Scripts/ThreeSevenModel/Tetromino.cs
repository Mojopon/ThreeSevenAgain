using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace ThreeSeven.Model
{
    public class Tetromino
    {
        public enum PolyominoIndex
        {
            None, I, N, Z, O, J, T, L
        }

        private Dictionary<PolyominoIndex, bool[,]> _table = null;
        private Dictionary<PolyominoIndex, bool[,]> Table
        {
            get
            {
                if(_table == null)
                {
                    InitializeTable();
                }
                return _table;
            }
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
                {{ false, true , false, false },
                 { false, true , true , false },
                 { false, false, true , false },
                 { false, false, false, false }}
                },
            };
        }

    }
}
