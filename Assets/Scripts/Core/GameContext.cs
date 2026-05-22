using UnityEngine;

//게임 전체에 영향을 주는 중앙 효과 변수 모음
public static class GameContext
{
    //손님 관련
    public static float customerSpawnInterval = 5f;
    public static float customerPatienceMultiplier = 1f;
    public static float customerRetainChance = 0f;

    //골드 관련
    public static float foodPriceMultiplier = 1f;
    public static float tipChance = 0f;
    public static float artifactDropChanceBonus = 0f;

    //영업/UI 관련
    public static int sessionStartBonusGold = 0;
    
}
