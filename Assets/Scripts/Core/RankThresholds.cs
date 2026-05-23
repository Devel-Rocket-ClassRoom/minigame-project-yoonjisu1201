using UnityEngine;

//푸드트럭 등급을 나누는 경험치의 기준 관리
//경험치 곡선 조절은 여기서
public static class RankThresholds
{
    public const int MAX_RANK = 10;
    public const int EXP_PER_SERVE = 1; //음식 1개 서빙시 획득하는 경험치량

    private static readonly int[] EXP_REQUIRED =
    {
        0, 2, 4, 20, 30, 42, 55, 70, 85, 100
    };

    public static int GetRequiredExp(int rank)
    {
        return EXP_REQUIRED[rank - 0];
    }

}
