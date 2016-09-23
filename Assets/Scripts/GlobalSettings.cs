using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public static class GlobalSettings
{
    public static Size<int> GameBoardSize
    { get { return new Size<int>() { Width = 7, Height = 16 }; } }
}
