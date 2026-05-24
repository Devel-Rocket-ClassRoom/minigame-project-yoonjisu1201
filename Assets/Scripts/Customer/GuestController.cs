using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//손님 이동, 인내심 타이머 관리
public class GuestController : MonoBehaviour
{
    [SerializeField] private GhostSO _ghostData;
    [SerializeField] private OrderPopup _orderPopup;
    [SerializeField] private float _entrySpeed = 2.5f;
    [SerializeField] private float _exitSpeed = 5f;
    [SerializeField] private bool _defaultFacingLeft = false;
    [SerializeField] private ContentRegistrySO _registry;

    public event System.Action OnExited; //GuestSpawner가 구독
    private const float SIGNATURE_ORDER_CHANCE = 0.8f;
    private SpriteRenderer _renderer;
    private Coroutine _patienceCoroutine;

    private Vector3 _stopPos;
    private Vector3 _exitPos;
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
        transform.position = entryPos;

        if (_defaultFacingLeft) 
            _renderer.flipX = entryPos.x < stopPos.x;
        else 
            _renderer.flipX = entryPos.x > stopPos.x;

        StartCoroutine(CoEntryRoutine());
    }
    private IEnumerator CoEntryRoutine()
    {
        while (Vector2.Distance(transform.position, _stopPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _stopPos,
                _entrySpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = _stopPos;
        //이동이 멈춘 다음에 주문팝업 노출
        CurrentOrder = PickOrder();
        _orderPopup.Show(CurrentOrder);
        _patienceCoroutine = StartCoroutine(CoPatienceRoutine());
    }
    private RecipeSO PickOrder()
    {
        List<RecipeSO> unlockedRecipes = new List<RecipeSO>();
        RecipeSO signatureRecipe = null;
        foreach (var recipe in _registry.allRecipes)
        {
            if (UnlockManager.instance.IsRecipeUnlocked(recipe))
                unlockedRecipes.Add(recipe);
        }
        if (unlockedRecipes.Count == 0) return null;

        foreach (var recipe in unlockedRecipes)
        {
            if (recipe.isSignatureMenu && recipe.ownerGhost == GhostData)
            {
                signatureRecipe = recipe;
                break;
            }
        }

        if (signatureRecipe != null && Random.value < SIGNATURE_ORDER_CHANCE)
            return signatureRecipe;

        return unlockedRecipes[Random.Range(0, unlockedRecipes.Count)];


    }
    //손님 퇴장, 이벤트로 DraggableFood에서 구독
    private IEnumerator CoExitRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (_defaultFacingLeft)
            _renderer.flipX = transform.position.x < _exitPos.x;
        else
            _renderer.flipX = transform.position.x > _exitPos.x;

        while (Vector2.Distance(transform.position, _exitPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _exitPos,
                _exitSpeed * Time.deltaTime);
            yield return null;
        }
        OnExited?.Invoke();
    }
    private IEnumerator CoPatienceRoutine()
    {
        float totalTimer = _ghostData.patienceSeconds * GameContext.customerPatienceMultiplier;
        float timer = totalTimer;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            _orderPopup.SetGauge(timer / totalTimer);
            yield return null;
        }

        _patienceCoroutine = null;
        CurrentOrder = null;
        _orderPopup.Hide();
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
        StartCoroutine(CoExitRoutine());
    }

}
