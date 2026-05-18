using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Scriptable Objects/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    //나중에 대화시스템 추가할때 사용할 SO
    //다른 SO들이 자기의 id를 가지고있고 DialogueSO로 와서 id를 찾아서 참조함
    public string id; 
    public string speakerName;
    public string[] lines;
}
