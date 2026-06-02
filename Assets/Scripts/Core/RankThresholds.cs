using UnityEngine;

//푸드트럭 등급을 나누는 경험치의 기준 관리
//경험치 곡선 조절은 여기서
public static class RankThresholds
{
    public const int MAX_RANK = 10;
    public const int EXP_PER_SERVE = 1;

    // 인덱스 = 현재 등급
    // rank 1 -> 2등급 필요 EXP = 5
    // rank 2 -> 3등급 필요 EXP = 12
    private static readonly int[] EXP_REQUIRED =
    {
        0,   // 사용 안 함
        5,   // 1 -> 2
        12,  // 2 -> 3
        20,  // 3 -> 4
        30,  // 4 -> 5
        42,  // 5 -> 6
        55,  // 6 -> 7
        70,  // 7 -> 8
        95,  // 8 -> 9
        120  // 9 -> 10
    };

    public static int GetRequiredExp(int currentRank)
    {
        if (currentRank >= MAX_RANK)
            return EXP_REQUIRED[MAX_RANK - 1];

        return EXP_REQUIRED[currentRank];
    }
}
