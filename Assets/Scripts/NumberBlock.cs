using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class NumberBlock : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _NumberSprites;

	public void SetNumber(int number)
    {
        GetComponent<SpriteRenderer>().sprite = _NumberSprites[number];
    }
}
