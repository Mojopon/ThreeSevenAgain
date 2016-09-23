using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public interface ISceneGameBoard
{
    void SetSize(Size<int> size);
    void AddTetromino(ThreeSevenBlock[] blocks);
    void MoveTetromino(Point<int>[] points);
    void DestroyTetromino();
    void PlaceBlock(Point<int> point, ThreeSevenBlock type);
    void MoveBlock(Point<int> source, Point<int> destination);
    void DeleteBlock(Point<int> point);

}
