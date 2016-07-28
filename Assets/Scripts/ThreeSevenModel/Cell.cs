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
            set
            {
                _block = (Block)value;
            }
        }
    }
}
