using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance {  get; private set; }

    public int TotalGold { get; private set; } = 0;
    public int SessionGold {  get; private set; }
    public event System.Action OnGoldChanged;

    private void Awake()
    {
        //싱글톤 중복 방지. 씬 전환 시 중복 생성 방어용
        if (Instance != null)
        {
            Destroy(gameObject); //원래 있던걸 두고 새로생긴걸 삭제
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AddGold(int amount)
    {
        TotalGold += amount;
        SessionGold += amount;
        OnGoldChanged?.Invoke();
    }

    public bool TrySpendGold(int amount)
    {
        if (TotalGold >= amount)
        {
            TotalGold -= amount;
            OnGoldChanged?.Invoke();
            return true;
        }
        return false;
    }
    public void ResetSession()
    {
        SessionGold = 0;
    }

}
