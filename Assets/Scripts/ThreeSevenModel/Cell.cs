using System.Collections;

namespace ThreeSeven.Model
{
    public class Cell
    {
        private Block _block;

        public IBlock Block
        {
            get
            {
                if(_block == null)
                {
                    return new NoBlock();
                }

                return _block;
            }
        }

        public void Set(IBlock block) { _block = (Block)block; }
        public bool IsNull { get { return _block == null; } }

        public void Clear()
        {
            _block = null;
        }
    }
}
