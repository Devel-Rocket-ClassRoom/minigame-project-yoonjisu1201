using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GuestSpawner : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;
    [SerializeField] private List<GuestController> _guestPrefabs;
    [SerializeField] private GuestController _tutorialGuestPrefab;
    [SerializeField] private Transform _entryPoint;
    [SerializeField] private Transform _stopPoint;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private RecipeSO _tutorialRecipe;
    public float _startSpawnDelay;


    private bool _isGuestPresent; //현재 손님이 있는지
    private bool _isStart = true; //처음 시작할때만 생성되는 딜레이시간 다르게
    private List<GuestController> _sessionPrefabs = new List<GuestController>();
    private List<RecipeSO> _sessionRecipes = new List<RecipeSO>();
    private Coroutine _spawnLoopCoroutine;

    private GuestController _currentGuest;
    private bool _firstGuestSpawned;

    public static event System.Action<GuestController> OnGuestSpawned;
    public bool HasOrderingGuest => _currentGuest != null &&
        _currentGuest.State == GuestState.Ordering;

    public bool IsGuestPresent => _isGuestPresent;
    private void Start()
    {
        foreach (var prefab in _guestPrefabs)
        {
            if (UnlockManager.instance.IsGhostUnlocked(prefab.GhostData))
                _sessionPrefabs.Add(prefab);
        }
        foreach (var recipe in _registry.allRecipes)
        {
            if (UnlockManager.instance.IsRecipeUnlocked(recipe))
                _sessionRecipes.Add(recipe);
        }
    }
    public void StartSpawning()
    {
        if (_spawnLoopCoroutine != null)
            StopCoroutine(_spawnLoopCoroutine);

        _spawnLoopCoroutine = StartCoroutine(CoSpawnLoop());
    }
    public void StopSpawning()
    {
        if (_spawnLoopCoroutine != null)
        {
            StopCoroutine(_spawnLoopCoroutine);
            _spawnLoopCoroutine = null;
        }
        _isStart = true;
    }
    private IEnumerator CoSpawnLoop()
    {
        if (_isStart)
        {
            _isStart = false;
            yield return new WaitForSeconds(_startSpawnDelay);
        }

        // 가이드 미완료 시 튜토리얼 손님 먼저 스폰
        if (_tutorialGuestPrefab != null && PlayerPrefs.GetInt("guide_cooking_done", 0) == 0)
        {
            SpawnGuest(_tutorialGuestPrefab);
            _currentGuest.SetSessionRecipes(new List<RecipeSO> { _tutorialRecipe });
            yield return new WaitUntil(() => !_isGuestPresent);
        }

        while (true) //영업마감 오픈 시스템 만들기 전까지 true로 사용
        {
            if (_sessionPrefabs.Count == 0)
            {
                yield return null;
                continue;
            }

            SpawnGuest(_sessionPrefabs[Random.Range(0, _sessionPrefabs.Count)]);

            yield return new WaitUntil(() => !_isGuestPresent);
            yield return new WaitForSeconds(GameContext.customerSpawnInterval);
        }
    }
    public void ForceExitEnteringGuest()
    {
        if (_currentGuest != null && _currentGuest.State == GuestState.Entering)
            _currentGuest.ForceExit();
    }
    private void SpawnGuest(GuestController prefab)
    {
        GuestController guest = Instantiate(prefab);
        _currentGuest = guest;
        guest.SetSessionRecipes(_sessionRecipes);
        _isGuestPresent = true;

        System.Action onExited = null;
        onExited = () =>
        {
            Destroy(guest.gameObject);
            _isGuestPresent = false;
            _currentGuest = null;
            guest.OnExited -= onExited;
        };
        guest.OnExited += onExited;
        guest.Enter(_entryPoint.position, _stopPoint.position, _exitPoint.position);
        OnGuestSpawned?.Invoke(guest);
    }
}
