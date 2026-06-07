using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Scriptable Objects/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public string id;
    public string[] speakerNames;
    public Sprite[] speakerSprites;
    public int[] lineSpeakerIndices;
}
