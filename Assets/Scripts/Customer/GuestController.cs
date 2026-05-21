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


    public event System.Action OnExited; //GuestSpawner가 구독
    private Coroutine _patienceCoroutine;

    private Vector3 _stopPos;
    private Vector3 _exitPos;
    public GhostSO GhostData => _ghostData;
    public RecipeSO CurrentOrder {  get; private set; }
    //손님 입장. GuestSpawner에서 호출
    public void Enter(Vector3 entryPos, Vector3 stopPos, Vector3 exitPos)
    {
        _stopPos = stopPos;
        _exitPos = exitPos;
        transform.position = entryPos;
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
        CookingSlot slot = CookingSlotManager.Instance.ActiveSlot;
        //만약 나중에 빈공간 눌렀을때 활성화된 슬롯이 없게 만든다면 이 로직도 수정
        if (slot == null) return null;  

        List<RecipeSO> candidates = slot.AllRecipes.ToList();
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }
    //손님 퇴장, 이벤트로 DraggableFood에서 구독
    private IEnumerator CoExitRoutine()
    {
        yield return new WaitForSeconds(1f);
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
        float timer = _ghostData.patienceSeconds;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            _orderPopup.SetGauge(timer / _ghostData.patienceSeconds);
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
