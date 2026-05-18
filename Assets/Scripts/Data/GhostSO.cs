using UnityEngine;

[CreateAssetMenu(fileName = "NewGhost", menuName = "Scriptable Objects/GhostSO")]
public class GhostSO : ScriptableObject
{
    // id, 이름, 이미지, 전용메뉴, 유물, 인내심, 해금레벨
    public string id;
    public string displayName;
    public Sprite icon;
    public float patienceSeconds;
    public int unlockRank;
    public ArtifactSO artifact;
    public string firstMeetDialogueID; //임시(대화)
    public string memoirDialogueID; //방명록id
}
