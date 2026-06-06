using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//손님 이동, 인내심 타이머 관리
public enum GuestState { Entering, Ordering, Exiting }
public class GuestController : MonoBehaviour
{
    [SerializeField] private BalanceConfigSO _balanceConfig;

    [SerializeField] private GhostSO _ghostData;
    [SerializeField] private OrderPopup _orderPopup;
    [SerializeField] private float _entrySpeed = 2.5f;
    [SerializeField] private float _entryBobAmplitude = 0.8f;
    [SerializeField] private float _entryBobSpeed = 8f;
    [SerializeField] private float _exitSpeed = 5f;
    [SerializeField] private bool _defaultFacingLeft = false;
    [SerializeField] private int _stoppedSortingOrder = 0;


    public event System.Action OnExited; //GuestSpawner가 구독
    public static event System.Action<GuestController> OnGuestOrdering;

    private static int _consecutiveSatisfiedCount = 0;
    public static void ResetStreak() => _consecutiveSatisfiedCount = 0;

    private List<RecipeSO> _sessionRecipes = new List<RecipeSO>();
    [SerializeField] private SpriteRenderer _renderer;
    private Coroutine _patienceCoroutine;
    private Coroutine _entryCoroutine;
    private bool _isPaused;

    public void Pause()
    {
        _isPaused = true;
        _orderPopup.Pause();
    }
    public void Resume()
    {
        _isPaused = false;
        _orderPopup.Resume();
    }

    public GuestState State { get; private set; }

    private Vector3 _stopPos;
    private Vector3 _exitPos;
    private Vector3 _entryPos;
    public Vector3 StopPos => _stopPos;
    public GhostSO GhostData => _ghostData;
    public RecipeSO CurrentOrder { get; private set; }
    //손님 입장. GuestSpawner에서 호출
    private void Awake()
    {
    }

    public void Enter(Vector3 entryPos, Vector3 stopPos, Vector3 exitPos)
    {
        _stopPos = stopPos;
        _exitPos = exitPos;
        _entryPos = entryPos;
        transform.position = entryPos;

        _renderer.sprite = _ghostData.spriteDefault;

        if (_defaultFacingLeft)
            _renderer.flipX = entryPos.x < stopPos.x;
        else
            _renderer.flipX = entryPos.x > stopPos.x;

        State = GuestState.Entering;
        _entryCoroutine = StartCoroutine(CoEntryRoutine());
    }
    private IEnumerator CoEntryRoutine()
    {
        _renderer.sortingOrder = _stoppedSortingOrder - 10; // 이동 중 뒤로
        float bobTime = 0f;
        Vector3 basePos = transform.position;

        while (Vector2.Distance(basePos, _stopPos) > 0.05f)
        {
            if (!_isPaused)
            {
                bobTime += Time.deltaTime;
                basePos = Vector2.MoveTowards(basePos, _stopPos, _entrySpeed * Time.deltaTime);
                float bobOffset = Mathf.Sin(bobTime * _entryBobSpeed) * _entryBobAmplitude;
                transform.position = new Vector3(basePos.x, basePos.y + bobOffset, basePos.z);
            }
            yield return null;
        }
        transform.position = _stopPos;
        _renderer.sortingOrder = _stoppedSortingOrder; // 도착 후 앞으로
        //이동이 멈춘 다음에 주문팝업 노출
        CurrentOrder = PickOrder();
        _orderPopup.Show(CurrentOrder, _ghostData);
        State = GuestState.Ordering;
        OnGuestOrdering?.Invoke(this);

        _patienceCoroutine = StartCoroutine(CoPatienceRoutine());
        _entryCoroutine = null;
    }
    private RecipeSO PickOrder()
    {
        if (_sessionRecipes.Count == 0) return null;

        RecipeSO signatureRecipe = null;

        foreach (var recipe in _sessionRecipes)
        {
            if (recipe.isSignatureMenu && recipe.ownerGhost == GhostData)
            {
                signatureRecipe = recipe;
                break;
            }
        }

        if (signatureRecipe != null && Random.value < _balanceConfig.signatureOrderChance)
            return signatureRecipe;

        return _sessionRecipes[Random.Range(0, _sessionRecipes.Count)];
    }
    //손님 퇴장, 이벤트로 DraggableFood에서 구독
    private IEnumerator CoExitRoutine()
    {
        _renderer.sortingOrder = _stoppedSortingOrder - 10; // 퇴장 시작하면 뒤로
        float bobTime = 0f;
        Vector3 basePos = transform.position;

        float delay = 0f;
        while (delay < 0.5f)
        {
            if (!_isPaused) delay += Time.deltaTime;
            yield return null;
        }

        if (_defaultFacingLeft)
            _renderer.flipX = transform.position.x < _exitPos.x;
        else
            _renderer.flipX = transform.position.x > _exitPos.x;

        while (Vector2.Distance(transform.position, _exitPos) > 0.05f)
        {
            if (!_isPaused)
            {
                bobTime += Time.deltaTime;
                basePos = Vector2.MoveTowards(basePos, _exitPos, _exitSpeed * GameContext.exitSpeedMultiplier * Time.deltaTime);
                float bobOffset = Mathf.Sin(bobTime * _entryBobSpeed) * _entryBobAmplitude;
                transform.position = new Vector3(basePos.x, basePos.y + bobOffset, basePos.z);
            }
            yield return null;
        }
        OnExited?.Invoke();
    }
    private IEnumerator CoPatienceRoutine()
    {
        float totalTimer = _balanceConfig.basePatienceSeconds
            * _balanceConfig.GetPatienceMultiplier(_ghostData.patienceType)
            * GameContext.customerPatienceMultiplier;
        float timer = totalTimer;
        while (timer > 0f)
        {
            if (!_isPaused)
            {
                timer -= Time.deltaTime;
                float ratio = timer / totalTimer;
                _orderPopup.SetGauge(ratio);

                if (ratio <= 0.2f)
                    _renderer.sprite = _ghostData.spriteAngry;
                else if (ratio <= 0.5f)
                    _renderer.sprite = _ghostData.spriteHalf;
                else
                    _renderer.sprite = _ghostData.spriteDefault;
            }
            yield return null;
        }

        if (GameContext.patienceRefillChance > 0f && Random.value < GameContext.patienceRefillChance)
        {
            timer = totalTimer;
            _patienceCoroutine = StartCoroutine(CoPatienceRoutine());
            yield break;
        }

        _consecutiveSatisfiedCount = 0;
        _patienceCoroutine = null;
        CurrentOrder = null;
        _orderPopup.Hide();
        State = GuestState.Exiting;
        StartCoroutine(CoExitRoutine());
    }
    //DraggableFood에서 호출
    public void ReceiveFood()
    {
        if (_patienceCoroutine != null)
        {
            StopCoroutine(_patienceCoroutine);
            _patienceCoroutine = null;
        }

        if (GameContext.consecutiveSatisfiedRequired > 0)
        {
            _consecutiveSatisfiedCount++;
            if (_consecutiveSatisfiedCount >= GameContext.consecutiveSatisfiedRequired)
            {
                GoldManager.Instance.AddGold(GameContext.consecutiveSatisfiedBonus);
                _consecutiveSatisfiedCount = 0;
            }
        }

        _renderer.sprite = _ghostData.spriteHappy;
        CurrentOrder = null;
        _orderPopup.Hide();
        State = GuestState.Exiting;
        StartCoroutine(CoExitRoutine());
    }
    public void SetSessionRecipes(List<RecipeSO> recipes)
    {
        _sessionRecipes = recipes;
    }
    public void ForceExit()
    {
        if (State != GuestState.Entering) return;
        if (_entryCoroutine != null)
        {
            StopCoroutine(_entryCoroutine); 
            _entryCoroutine = null;
        }
        State = GuestState.Exiting;
        StartCoroutine(CoTurnAroundAndExit());
    }
    private IEnumerator CoTurnAroundAndExit()
    {
        _renderer.sortingOrder = _stoppedSortingOrder - 10;
        if (_defaultFacingLeft)
            _renderer.flipX = transform.position.x < _entryPos.x;
        else
            _renderer.flipX = transform.position.x > _entryPos.x;

        while (Vector2.Distance(transform.position, _entryPos) > 0.05f)
        {
            if (!_isPaused)
                transform.position = Vector2.MoveTowards(transform.position, _entryPos,
                    _entrySpeed * Time.deltaTime);
            yield return null;
        }
        OnExited?.Invoke();
    }
}
