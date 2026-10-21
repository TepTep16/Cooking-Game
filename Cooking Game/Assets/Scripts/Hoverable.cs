using UnityEngine;


public class Hoverable : MonoBehaviour
{
    [Tooltip("Name shown on the first line of the panel")]
    public string displayName = "Object";

    [Tooltip("Hint shown on the second line, e.g. \"LMB: place food\"")]
    [TextArea] public string hint;
}

