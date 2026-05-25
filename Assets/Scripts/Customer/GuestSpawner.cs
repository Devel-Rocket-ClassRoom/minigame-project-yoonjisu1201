using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GuestSpawner : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;
    [SerializeField] private List<GuestController> _guestPrefabs;
    [SerializeField] private Transform _entryPoint;
    [SerializeField] private Transform _stopPoint;
    [SerializeField] private Transform _exitPoint;
    public float _startSpawnDelay;

    private bool _isGuestPresent; //현재 손님이 있는지
    private bool _isStart = true; //처음 시작할때만 생성되는 딜레이시간 다르게
    private List<GuestController> _sessionPrefabs = new List<GuestController>();
    private List<RecipeSO> _sessionRecipes = new List<RecipeSO>();
    private Coroutine _spawnLoopCoroutine;
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
        _spawnLoopCoroutine = StartCoroutine(CoSpawnLoop());
    }
    public void StopSpawning()
    {
        if (_spawnLoopCoroutine != null)
        {
            StopCoroutine(_spawnLoopCoroutine);
            _spawnLoopCoroutine = null;
        }
    }
    private IEnumerator CoSpawnLoop()
    {
        while (true) //영업마감 오픈 시스템 만들기 전까지 ture로 사용
        {
            if (_isStart)
            {
                _isStart = false;
                yield return new WaitForSeconds(_startSpawnDelay);
            }
            else
            {
                yield return new WaitForSeconds(GameContext.customerSpawnInterval);
            }

            if (_sessionPrefabs.Count == 0) continue;

            GuestController guest = Instantiate(_sessionPrefabs[Random.Range(0, _sessionPrefabs.Count)]);
            guest.SetSessionRecipes(_sessionRecipes);

            _isGuestPresent = true;

            System.Action onExited = null;
            onExited = () =>
            {
                Destroy(guest.gameObject);
                _isGuestPresent = false;
                guest.OnExited -= onExited;
            };
            guest.OnExited += onExited;

            guest.Enter(_entryPoint.position, _stopPoint.position, _exitPoint.position);

            yield return new WaitUntil(() => !_isGuestPresent);
        }
    }
}
