using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    [SerializeField] private float _sessionDuration = 5f; //한 세션 2분
    [SerializeField] private List<GuestSpawner> _spawners;
    [SerializeField] private GameObject _closingPanel;

    public static SessionManager instance { get; private set; }
    public float SessionDuration => _sessionDuration;
    public float RemainingTime { get; private set; }
    public bool IsSessionActive { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        RemainingTime = _sessionDuration;
        IsSessionActive = true;
        StartCoroutine(CoSessionTimer());
        GoldManager.Instance.ResetSession();
    }
    private IEnumerator CoSessionTimer()
    {
        while (RemainingTime > 0f)
        {
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
        }
        StartCoroutine(CoWaitForGuestsAndClose());
    }
    private IEnumerator CoWaitForGuestsAndClose()
    {
        //All -> List의 모든 원소가 조건을 만족해야한다.
        yield return new WaitUntil(() => _spawners.All(s => !s.IsGuestPresent));
        _closingPanel.SetActive(true);
    }

}
