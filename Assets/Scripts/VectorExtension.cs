using UnityEngine;
using System.Collections;

public static class VectorExtension
{
    public static Vector3 InvertYAxis(this Vector3 @this)
    {
        return new Vector3(@this.x, -@this.y, @this.z);
    }
}
