using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ByteDataConversion
{
    public static byte[] AddTetrominoToByte(ThreeSevenBlock[] blocks)
    {
        List<byte> byteList = new List<byte>();

        foreach(var block in blocks)
        {
            byteList.Add((byte)block);
        }

        return byteList.ToArray();
    }
}
