using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SceneBlock : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _NumberSprites;

	public void SetNumber(int number)
    {
        GetComponent<SpriteRenderer>().sprite = _NumberSprites[number];
    }

    public void MoveTo(Vector3 destination)
    {
        //transform.localPosition = destination;

        iTween.MoveTo(gameObject, iTween.Hash("x", destination.x, "y", destination.y, "speed", 25f, "isLocal", true, "easeType", "easeInQuad"));

    }
}
