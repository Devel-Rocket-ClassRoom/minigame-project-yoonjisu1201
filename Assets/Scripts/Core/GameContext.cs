using UnityEngine;

//게임 전체에 영향을 주는 중앙 효과 변수 모음
public static class GameContext
{
    // artifact_2: 퇴장 속도 배율 (1f + effectValue)
    public static float exitSpeedMultiplier = 1f;

    // artifact_3: 망한 음식 위로금
    public static float consolationChance = 0f;
    public static int consolationGold = 0;

    // artifact_4: 음식 상하는 시간 배율 (1f + effectValue)
    public static float spoilTimeMultiplier = 1f;

    // artifact_5: 인내심 배율 (1f + effectValue)
    public static float customerPatienceMultiplier = 1f;

    // artifact_6: 영업 종료 시 서빙 횟수당 보너스 골드
    public static int sessionEndBonusGoldPerDish = 0;

    // artifact_7: 슬롯별 첫 조리 속도 부스트 배율 (1f = 없음, 0.2 = 80% 빠름)
    public static float firstCookSpeedMultiplier = 1f;

    // artifact_8: 느긋한 손님 등장 확률 + 인내심 배율
    public static float relaxedCustomerChance = 0f;
    public static float relaxedCustomerPatienceMultiplier = 1f;

    // artifact_9: 인내심 소진 시 리필 확률
    public static float patienceRefillChance = 0f;
}
