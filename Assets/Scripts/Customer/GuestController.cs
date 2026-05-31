using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//손님 이동, 인내심 타이머 관리
public enum GuestState { Entering, Ordering, Exiting }
public class GuestController : MonoBehaviour
{
    [SerializeField] private GhostSO _ghostData;
    [SerializeField] private OrderPopup _orderPopup;
    [SerializeField] private float _entrySpeed = 2.5f;
    [SerializeField] private float _entryBobAmplitude = 0.8f;
    [SerializeField] private float _entryBobSpeed = 8f;
    [SerializeField] private float _exitSpeed = 5f;
    [SerializeField] private bool _defaultFacingLeft = false;
    [SerializeField] private int _stoppedSortingOrder = 0;


    public event System.Action OnExited; //GuestSpawner가 구독
    private const float SIGNATURE_ORDER_CHANCE = 0.5f;
    private List<RecipeSO> _sessionRecipes = new List<RecipeSO>();
    private SpriteRenderer _renderer;
    private Coroutine _patienceCoroutine;
    private Coroutine _entryCoroutine;

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
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Enter(Vector3 entryPos, Vector3 stopPos, Vector3 exitPos)
    {
        _stopPos = stopPos;
        _exitPos = exitPos;
        _entryPos = entryPos;
        transform.position = entryPos;

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
            bobTime += Time.deltaTime;
            basePos = Vector2.MoveTowards(basePos, _stopPos, _entrySpeed * Time.deltaTime);
            float bobOffset = Mathf.Sin(bobTime * _entryBobSpeed) * _entryBobAmplitude;
            transform.position = new Vector3(basePos.x, basePos.y + bobOffset, basePos.z);
            yield return null;
        }
        transform.position = _stopPos;
        _renderer.sortingOrder = _stoppedSortingOrder; // 도착 후 앞으로
        //이동이 멈춘 다음에 주문팝업 노출
        CurrentOrder = PickOrder();
        _orderPopup.Show(CurrentOrder, _ghostData);
        State = GuestState.Ordering;
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

        if (signatureRecipe != null && Random.value < SIGNATURE_ORDER_CHANCE)
            return signatureRecipe;

        return _sessionRecipes[Random.Range(0, _sessionRecipes.Count)];
    }
    //손님 퇴장, 이벤트로 DraggableFood에서 구독
    private IEnumerator CoExitRoutine()
    {
        _renderer.sortingOrder = _stoppedSortingOrder - 10; // 퇴장 시작하면 뒤로
        float bobTime = 0f;
        Vector3 basePos = transform.position;
        yield return new WaitForSeconds(0.5f);

        if (_defaultFacingLeft)
            _renderer.flipX = transform.position.x < _exitPos.x;
        else
            _renderer.flipX = transform.position.x > _exitPos.x;

        while (Vector2.Distance(transform.position, _exitPos) > 0.05f)
        {
            bobTime += Time.deltaTime;
            basePos = Vector2.MoveTowards(basePos, _exitPos, _exitSpeed * Time.deltaTime);
            float bobOffset = Mathf.Sin(bobTime * _entryBobSpeed) * _entryBobAmplitude;
            transform.position = new Vector3(basePos.x, basePos.y + bobOffset, basePos.z);
            yield return null;
        }
        OnExited?.Invoke();
    }
    private IEnumerator CoPatienceRoutine()
    {
        float totalTimer = _ghostData.patienceSeconds * GameContext.customerPatienceMultiplier;
        float timer = totalTimer;  //테스트하느라 임시설정
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            _orderPopup.SetGauge(timer / totalTimer);
            yield return null;
        }

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
            transform.position = Vector2.MoveTowards(transform.position, _entryPos,
                _entrySpeed * Time.deltaTime);
            yield return null;
        }
        OnExited?.Invoke();
    }
}
