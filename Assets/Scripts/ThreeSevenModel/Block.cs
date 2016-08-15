using System;
using System.Collections;

namespace ThreeSeven.Model
{
    public enum ThreeSevenBlock
    {
        None,
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
        ThreeSevenBlock Type { get; }

        int GetNumber();
    }

    public class Block : IBlock
    {
        public ThreeSevenBlock Type { get; private set; }
        
        private Block(ThreeSevenBlock blockType)
        {
            Type = blockType;
        }

        public int GetNumber()
        {
            if(Type > 0 && 7 >= (int)Type)
            {
                return (int)Type;
            }

            return 0;
        }

        public static Block Create()
        {
            return Create(new Random());
        }

        public static Block Create(Random random)
        {
            return Create((ThreeSevenBlock)random.Next(1, 7 + 1));
        }

        public static Block Create(ThreeSevenBlock blockType)
        {
            return new Block(blockType);
        }
    }

    public class NoBlock : IBlock
    {
        public ThreeSevenBlock Type { get { return ThreeSevenBlock.None; } }

        public int GetNumber() { return 0; }
    }
}