using UnityEngine;

public enum ArtifactEffectType
{
    None,
    ExitSpeedBoost,         // artifact_2: 퇴장 속도 증가
    FailedFoodConsolation,  // artifact_3: 망한 음식 위로금
    SpoilTimeExtension,     // artifact_4: 음식 상하는 시간 연장
    PatienceBoost,          // artifact_5: 인내심 증가
    SessionEndBonus,        // artifact_6: 영업 종료 보너스 골드
    FirstCookSpeedBoost,    // artifact_7: 첫 조리 속도 부스트
    ConsecutiveSatisfactionBonus,  // artifact_8: N명 연속 만족 시 보너스 골드
    PatienceRefill,                // artifact_9: 인내심 소진 시 리필 확률
}

[CreateAssetMenu(fileName = "ArtifactSO", menuName = "Scriptable Objects/ArtifactSO")]
public class ArtifactSO : ScriptableObject
{
    public string id;
    public Sprite icon;
    public Sprite passiveIcon;

    public ArtifactEffectType effectType;
    [Tooltip("주요 효과 수치\n" +
             "2:퇴장속도배율증가(0.15) / 3:위로금확률(0.3) / 4:상하는시간배율증가(0.15)\n" +
             "5:인내심배율증가(0.05) / 6:서빙당보너스골드(5) / 7:첫조리배율(0.2=80%빠름)\n" +
             "8:연속만족필요횟수(3) / 9:인내심리필확률(0.5)")]
    public float effectValue;
    [Tooltip("보조 수치 (artifact_3: 위로금 골드량 / artifact_8: 연속만족 보너스 골드량)")]
    public float effectValue2;
}
