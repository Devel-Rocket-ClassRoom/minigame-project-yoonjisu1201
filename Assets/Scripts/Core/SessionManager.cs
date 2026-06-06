using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    [SerializeField] private BalanceConfigSO _balanceConfig;

    [SerializeField] private List<GuestSpawner> _spawners;
    [SerializeField] private GameObject _closingPanel;

    public static SessionManager instance { get; private set; }
    public float SessionDuration => _balanceConfig.sessionDuration;
    public float RemainingTime { get; private set; }
    public bool IsSessionActive { get; private set; }

    private Coroutine _sessionTimerCoroutine;
    private bool _timerPaused;
    private int _sessionServeCount = 0;
    private int _sessionGuestCount = 0;
    public int SessionGuestCount => _sessionGuestCount;

    public void PauseTimer() => _timerPaused = true;
    public void ResumeTimer() => _timerPaused = false;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        DraggableFood.OnAnyServeSuccess += OnServeSuccess;
        GuestSpawner.OnGuestSpawned += OnGuestSpawned;
    }
    private void OnDisable()
    {
        DraggableFood.OnAnyServeSuccess -= OnServeSuccess;
        GuestSpawner.OnGuestSpawned -= OnGuestSpawned;
    }
    private void OnServeSuccess() => _sessionServeCount++;
    private void OnGuestSpawned(GuestController _) => _sessionGuestCount++;
    private void Start()
    {
        RemainingTime = _balanceConfig.sessionDuration;
        IsSessionActive = true;

        GoldManager.Instance.ResetSession();
        TruckRankManager.instance.ResetSession();

        PreparedRecipeManager.Instance.BeginPrepareFlow();
    }
    public void StartSessionAfterPrepare()
    {
        RemainingTime = _balanceConfig.sessionDuration;
        IsSessionActive = true;

        foreach (var spawner in _spawners)
            spawner.StartSpawning();

        if (_sessionTimerCoroutine != null)
            StopCoroutine(_sessionTimerCoroutine);

        _sessionTimerCoroutine = StartCoroutine(CoSessionTimer());
    }
    private IEnumerator CoSessionTimer()
    {
        while (RemainingTime > 0f)
        {
            if (!_timerPaused)
                RemainingTime -= Time.deltaTime;

            yield return null;
        }
        RemainingTime = 0f;
        OnTimerExpired();
    }
    private void OnTimerExpired() //Expired 만료
    {
        IsSessionActive = false;
        foreach (var spawner in _spawners)
        {
            spawner.StopSpawning();
            spawner.ForceExitEnteringGuest();
        }

        if (GameContext.sessionEndBonusGoldPerDish > 0 && _sessionServeCount > 0)
        {
            int bonus = GameContext.sessionEndBonusGoldPerDish * _sessionServeCount;
            GoldManager.Instance.AddGold(bonus);
        }

        StartCoroutine(CoWaitForGuestsAndClose());
    }
    private IEnumerator CoWaitForGuestsAndClose()
    {
        //All -> List의 모든 원소가 조건을 만족해야한다.
        yield return new WaitUntil(() => _spawners.All(s => !s.HasOrderingGuest));
        _closingPanel.SetActive(true);
    }

}
