using UnityEngine;

[System.Serializable]
public struct SpawnConfig
{
    public float spawnInterval;
    [Tooltip("스포너 인덱스별 딜레이 배수 (스포너1=인덱스0, 스포너2=인덱스1, 스포너3=인덱스2)")]
    public float[] startDelayMultipliers;
}
[CreateAssetMenu(fileName = "BalanceConfigSO", menuName = "Scriptable Objects/BalanceConfigSO")]
public class BalanceConfigSO : ScriptableObject
{
    [Header("세션")]
    public float sessionDuration = 120f;

    [Tooltip("조리슬롯 레벨별 스폰 설정 (인덱스0=레벨0, 인덱스1=레벨1, 인덱스2=레벨2)")]
    public SpawnConfig[] spawnBySlotLevel = new SpawnConfig[]
    {
        new SpawnConfig { spawnInterval = 6.5f, startDelayMultipliers = new float[] { 1f, 2f, 3f } },
        new SpawnConfig { spawnInterval = 5.5f, startDelayMultipliers = new float[] { 1f, 1.8f, 2.8f } },
        new SpawnConfig { spawnInterval = 5f,   startDelayMultipliers = new float[] { 1f, 1.5f, 2f } },
    };
    [Header("손님")]
    [Range(0f, 1f)] public float signatureOrderChance = 0.45f;
    [Range(0f, 1f)] public float artifactDropChance = 0.15f;

    [Header("인내심")]
    public float basePatienceSeconds = 28f;
    public float patienceRelaxedMultiplier = 1.15f;
    public float patienceNormalMultiplier = 1.0f;
    public float patienceHastyMultiplier = 0.9f;
    public float patienceHurriedMultiplier = 0.8f;

    public float GetPatienceMultiplier(PatienceType type) => type switch
    {
        PatienceType.Relaxed  => patienceRelaxedMultiplier,
        PatienceType.Hasty    => patienceHastyMultiplier,
        PatienceType.Hurried  => patienceHurriedMultiplier,
        _                     => patienceNormalMultiplier,
    };

    [Header("음식")]
    public float foodSpoilTime = 15f;

    [Header("경험치")]
    public int expPerServe = 1;

    [Tooltip("인덱스 = 현재 등급. [0]은 사용 안 함. 1->2 필요EXP, 2->3 필요EXP ...")]
    public int[] rankExpThresholds = { 0, 12, 36, 70, 115, 170, 235, 310, 395, 490 };

    [Header("업그레이드 비용")]
    [Tooltip("조리 슬롯 추가 비용 (레벨 1->2, 2->3 순서)")]
    public int[] cookSlotCosts = { 450, 1400 };

    [Tooltip("조리 속도 업 비용 (레벨 1->2, 2->3, 3->4 순서)")]
    public int[] speedUpCosts = { 250, 700, 1600 };

    [Tooltip("조리속도 감소 배율 (레벨1: 0.9=10%감소, 레벨2: 0.8=20%감소, 레벨3: 0.7=30%감소)")]
    public float[] speedUpMultipliers = { 0.9f, 0.8f, 0.7f };

    [Tooltip("레시피 보조판 해금 비용")]
    public int[] cookBoardCosts = { 180, 550 };

    [Tooltip("주문팝업힌트 업그레이드 비용")]
    public int[] orderHintCosts = { 200, 600, 1300 };

    [Tooltip("재료보관 슬롯 추가 비용")]
    public int[] containerSlotCosts = { 350, 1000 };
}
