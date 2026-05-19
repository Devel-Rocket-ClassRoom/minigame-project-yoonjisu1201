using UnityEngine;
using UnityEngine.EventSystems;

//조리대 슬롯의 비주얼, 입력
[RequireComponent(typeof(Collider2D))] //필수 컴포넌트
public class CookingSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CookingSlot _slot; //얘가 로직담당
    [SerializeField] private SpriteRenderer _stateRenderer;
    [SerializeField] private SpriteRenderer _highlightRenderer;

    //상태별 스프라이트 -> 나중에 애니메이션 교체
    [SerializeField] private Sprite _spriteEmpty;
    [SerializeField] private Sprite _spriteFilling;
    [SerializeField] private Sprite _spriteCooking;
    [SerializeField] private Sprite _spriteReady;
    [SerializeField] private Sprite _spriteSpoiled;

    public CookingSlot Slot => _slot;
    private float _testCookTime = 5f;

    private void Awake()
    {
        _slot = GetComponent<CookingSlot>();
        _highlightRenderer.enabled = false;
    }
    private void OnEnable()
    {
        _slot.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        _slot.OnStateChanged -= HandleStateChanged;
    }
    public void OnPointerClick(PointerEventData eventData) //이건 꼭 퍼블릭으로
    {
        Debug.Log($"{gameObject.name} 클릭됨");
        CookingSlotManager.Instance.SetActiveSlot(this);
    }
    public void SetHighlight(bool active)
    {
        _highlightRenderer.enabled = active;
    }
    private void HandleStateChanged(CookingSlotState state)
    {
        _stateRenderer.sprite = state switch
        {
            CookingSlotState.Empty   => _spriteEmpty,
            CookingSlotState.Filling => _spriteFilling,
            CookingSlotState.Cooking => _spriteCooking,
            CookingSlotState.Ready   => _spriteReady,
            CookingSlotState.Spoiled => _spriteSpoiled,
            _                        => _spriteEmpty,
        };
    }
    public void OnStartCookingButtonClicked()
    {
        _slot.StartCooking(_testCookTime);
    }

}
