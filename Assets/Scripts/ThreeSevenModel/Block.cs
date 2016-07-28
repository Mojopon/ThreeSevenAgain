using System;
using System.Collections;

namespace ThreeSeven.Model
{
    public enum ThreeSevenBlock
    {
        NoBlock,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
    }

    public interface IBlock
    {
        ThreeSevenBlock BlockType { get; }

        int GetNumber();
    }

    public class Block : IBlock
    {
        static Random random = new Random();

        public ThreeSevenBlock BlockType { get; private set; }
        
        public Block(ThreeSevenBlock blockType)
        {
            BlockType = blockType;
        }

        public int GetNumber()
        {
            if(BlockType > 0 && 7 >= (int)BlockType)
            {
                return (int)BlockType;
            }

            return 0;
        }

        public static Block Create()
        {
            return new Block((ThreeSevenBlock)random.Next(1, 7 + 1));
        }
    }

    public class NoBlock : IBlock
    {
        public ThreeSevenBlock BlockType { get { return ThreeSevenBlock.NoBlock; } }

        public int GetNumber() { return 0; }
    }
}