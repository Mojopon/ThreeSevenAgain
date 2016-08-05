using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ThreeSeven.Model
{
    public enum PolyominoIndex
    {
        None, I, N, Z, J, T, L, ShortL, ShortI, ShortJ
    }

    public class Polyomino
    {
        public static Polyomino Create()
        {
            return Create(PolyominoIndex.I);
        }

        public static Polyomino Create(PolyominoIndex polyminoIndex)
        {
            return new Polyomino(Table[polyminoIndex]);
        }

        public Size<int> Size { get { return _size; } }
        public Point<int>[] Points { get { return _patterns[_currentPattern]; } }
        public int Length { get { return Points.Length; } }

        private int _currentPattern = 0;
        private List<Point<int>[]> _patterns = null;
        private Size<int> _size;

        private Polyomino(List<Point<int>[]> patterns)
        {
            _patterns = patterns;
            _size = GetSizeFromPatterns();
        }

        private Size<int> GetSizeFromPatterns()
        {
            var size = new Size<int> { Width = -1, Height = -1 };

            foreach(var points in _patterns)
            {
                foreach(var point in points)
                {
                    size.Width  = point.X > size.Width  ? point.X : size.Width;
                    size.Height = point.Y > size.Height ? point.Y : size.Height;
                }
            }

            if(size.Width != -1 || size.Height != -1)
            {
                size.Width++;
                size.Height++;
            }

            return size;
        }

        public void Turn()
        {
            Turn(true);
        }

        public void Turn(bool clockowise)
        {
            switch(clockowise)
            {
                case true:
                    _currentPattern++;
                    if(_currentPattern >= _patterns.Count)
                    {
                        _currentPattern = 0;
                    }
                    break;
                case false:
                    _currentPattern--;
                    if(0 > _currentPattern)
                    {
                        _currentPattern = _patterns.Count;
                    }
                    break;
            }
        }

        public override string ToString()
        {
            if (_patterns == null) return "No Points";

            StringBuilder builder = new StringBuilder();
            foreach(var point in Points)
            {
                builder.Append(string.Format("({0}, {1}) ", point.X, point.Y));
            }

            return builder.ToString();
        }

        public static Dictionary<PolyominoIndex, List<Point<int>[]>> Table
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

        private static Dictionary<PolyominoIndex, List<Point<int>[]>> _table;
        private static void InitializeTable()
        {
            _table = new Dictionary<PolyominoIndex, List<Point<int>[]>>()
            {
                { PolyominoIndex.I, new List<Point<int>[]>() {
                    new Point<int>[] {
                    new Point<int>() {X = 2, Y = 0 },
                    new Point<int>() {X = 2, Y = 1 },
                    new Point<int>() {X = 2, Y = 2 },
                    new Point<int>() {X = 2, Y = 3 },
                    },
                    new Point<int>[] {
                    new Point<int>() {X = 3, Y = 1 },
                    new Point<int>() {X = 2, Y = 1 },
                    new Point<int>() {X = 1, Y = 1 },
                    new Point<int>() {X = 0, Y = 1 },
                    },
                    new Point<int>[] {
                    new Point<int>() {X = 1, Y = 3 },
                    new Point<int>() {X = 1, Y = 2 },
                    new Point<int>() {X = 1, Y = 1 },
                    new Point<int>() {X = 1, Y = 0 },
                    },
                    new Point<int>[] {
                    new Point<int>() {X = 0, Y = 1 },
                    new Point<int>() {X = 1, Y = 1 },
                    new Point<int>() {X = 2, Y = 1 },
                    new Point<int>() {X = 3, Y = 1 },
                    },
                } },
                { PolyominoIndex.N, new List<Point<int>[]>() {
                    new Point<int>[] {
                    new Point<int>() {X = 1, Y = 0 },
                    new Point<int>() {X = 1, Y = 1 },
                    new Point<int>() {X = 0, Y = 1 },
                    new Point<int>() {X = 0, Y = 2 },
                    },
                    new Point<int>[] {
                    new Point<int>() {X = 2, Y = 1 },
                    new Point<int>() {X = 1, Y = 1 },
                    new Point<int>() {X = 1, Y = 0 },
                    new Point<int>() {X = 0, Y = 0 },
                    },
                    new Point<int>[] {
                    new Point<int>() {X = 1, Y = 2 },
                    new Point<int>() {X = 1, Y = 1 },
                    new Point<int>() {X = 2, Y = 1 },
                    new Point<int>() {X = 2, Y = 0 },
                    },
                    new Point<int>[] {
                    new Point<int>() {X = 0, Y = 1 },
                    new Point<int>() {X = 1, Y = 1 },
                    new Point<int>() {X = 1, Y = 2 },
                    new Point<int>() {X = 2, Y = 2 },
                    },
                } },
            };
        }
    }
}
