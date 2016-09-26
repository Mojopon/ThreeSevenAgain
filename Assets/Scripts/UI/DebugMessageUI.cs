using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugMessageUI : SingletonMonoBehaviour<DebugMessageUI>
{
    [SerializeField]
    private Text text;

    public void SetMessage(string message)
    {
        text.text = message;
    }
}
