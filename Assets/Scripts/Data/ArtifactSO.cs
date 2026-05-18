using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactSO", menuName = "Scriptable Objects/ArtifactSO")]
public class ArtifactSO : ScriptableObject
{
    //id, 이름, 이미지, 방명록 텍스트, 유물효과 설명 텍스트
    public string id;
    public string displayName;
    public Sprite icon;
    public string memoirText;
    public string functionalDesc;
}
