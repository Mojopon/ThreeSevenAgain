using UnityEngine;
using System.Collections;
using ThreeSeven.Model;

public class SceneTetromino
{
    public SceneBlock[] sceneBlocks;
    public Point<int>[] positions;

    public SceneTetromino(SceneBlock[] sceneBlocks)
    {
        this.sceneBlocks = sceneBlocks;
    }
}
