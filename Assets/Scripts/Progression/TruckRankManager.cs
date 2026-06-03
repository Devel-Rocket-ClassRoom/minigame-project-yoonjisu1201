using Unity.VisualScripting;
using UnityEngine;

//EXP / 등급 계산
public class TruckRankManager : MonoBehaviour
{
    public static TruckRankManager instance { get; private set; }

    [SerializeField] private BalanceConfigSO _balanceConfig;

    [Header("DEBUG")]
    [SerializeField] private int _debugStartRank = 1;

    public int CurrentRank { get; private set; } = 1;
    public float TotalExp { get; private set; } = 0f; 
    public float SessionExp { get; private set; } = 0f;

    public int MaxRank => _balanceConfig.rankExpThresholds.Length;
    public int ExpPerServe => _balanceConfig.expPerServe;
    public int GetRequiredExp(int rank) => _balanceConfig.rankExpThresholds[rank];
    //해금 관리하는곳에서 구독
    public event System.Action<int> OnRankUp;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        if (_debugStartRank > 1)
            CurrentRank = _debugStartRank;
#endif
    }

    public void AddExp(int amount)
    {
        TotalExp += amount;
        SessionExp += amount;
        CheckRankUp();
    }
    public void ResetSession()
    {
        SessionExp = 0f;
    }
    private void CheckRankUp()
    {
        while (CurrentRank < MaxRank)
        {
            if (TotalExp >= GetRequiredExp(CurrentRank))
            {
                CurrentRank++;
                OnRankUp?.Invoke(CurrentRank);
            }
            else break;
        }
    }
}
