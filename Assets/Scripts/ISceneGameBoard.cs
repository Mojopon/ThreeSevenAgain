using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public interface ISceneGameBoard
{
    void SetSize(Size<int> size);
    void AddTetromino(ThreeSevenBlock[] blocks);
    void MoveTetromino(Point<int>[] points);
}
