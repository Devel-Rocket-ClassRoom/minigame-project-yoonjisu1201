using Unity.VisualScripting;
using UnityEngine;

//EXP / 등급 계산
public class TruckRankManager : MonoBehaviour
{
    public static TruckRankManager instance { get; private set; }

    [Header("DEBUG")]
    [SerializeField] private int _debugStartRank = 1;

    public int CurrentRank { get; private set; } = 1;
    public float TotalExp { get; private set; } = 0f; 
    public float SessionExp { get; private set; } = 0f;

    //해금 관리하는곳에서 고둑
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
        while (CurrentRank < RankThresholds.MAX_RANK)
        {
            if (TotalExp >= RankThresholds.GetRequiredExp(CurrentRank))
            {
                CurrentRank++;
                OnRankUp?.Invoke(CurrentRank);
            }
            else break;
        }
    }
}
