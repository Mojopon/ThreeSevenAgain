using System.Collections;

namespace ThreeSeven.Model
{
    public enum ThreeSevenBlock
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
    }

    public class Block
    {
        public ThreeSevenBlock BlockType { get; private set; }
        
        public Block(ThreeSevenBlock blockType)
        {
            BlockType = blockType;
        }

        private int GetNumber()
        {
            if(BlockType > 0 && 7 >= (int)BlockType)
            {
                return (int)BlockType;
            }

            return 0;
        }
    }
}