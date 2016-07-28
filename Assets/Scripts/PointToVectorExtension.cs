using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public static class PointToVectorExtension
{
    public static Vector3 ToVector3(this Point<int> @this)
    {
        return new Vector3(@this.X, @this.Y, 0);
    }

    public static Vector3 ToVector3(this Point<int> @this, float zaxis)
    {
        return new Vector3(@this.X, @this.Y, zaxis);
    }

    public static Vector3 ToVector2(this Point<int> @this)
    {
        return new Vector2(@this.X, @this.Y);
    }
}
