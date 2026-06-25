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
    public int PendingRankUp { get; private set; } = 0;
    public event System.Action<int> OnRankUp;
    public event System.Action OnRankStateChanged;

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
        SessionExp += amount;
        OnRankStateChanged?.Invoke();
    }

    public void CommitSession()
    {
        TotalExp += SessionExp;
        CheckRankUp();
        OnRankStateChanged?.Invoke();
    }

    public void ResetSession()
    {
        SessionExp = 0f;
        OnRankStateChanged?.Invoke();
    }
    private void CheckRankUp()
    {
        while (CurrentRank < MaxRank)
        {
            if (TotalExp >= GetRequiredExp(CurrentRank))
            {
                CurrentRank++;
                PendingRankUp = CurrentRank;
                OnRankUp?.Invoke(CurrentRank);
            }
            else break;
        }
    }
    public void LoadFrom(SaveData data)
    {
        CurrentRank = data.currentRank;
        TotalExp = data.totalExp;
        SessionExp = 0f;
        OnRankStateChanged?.Invoke();
    }

    public void ClearPendingRankUp() => PendingRankUp = 0;

    public void ResetAll()
    {
        CurrentRank = 1;
        TotalExp = 0f;
        SessionExp = 0f;
        PendingRankUp = 0;
        OnRankStateChanged?.Invoke();
    }
}
