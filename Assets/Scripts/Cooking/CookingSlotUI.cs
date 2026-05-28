using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

//조리대 슬롯의 비주얼, 입력
[RequireComponent(typeof(Collider2D))] //필수 컴포넌트
public class CookingSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CookingSlot _slot; //얘가 로직담당
    [SerializeField] private SpriteRenderer _stateRenderer;
    [SerializeField] private SpriteRenderer _highlightRenderer;
    [SerializeField] private SpriteRenderer _resultRenderer;
    [SerializeField] private DraggableFood _draggableFood;
    [SerializeField] private IngredientPreviewEffect[] _ingredientPreviews;

    //상태별 스프라이트 -> 나중에 애니메이션 교체
    [SerializeField] private Sprite _spriteEmpty;
    [SerializeField] private Sprite _spriteFilling;
    [SerializeField] private Sprite _spriteCooking;
    [SerializeField] private Sprite _spriteReady;
    [SerializeField] private Sprite _spriteSpoiled;
    [SerializeField] private Sprite _spriteFail;
    [SerializeField] private float _resultSize = 1f;

    public CookingSlot Slot => _slot;
    private float _testCookTime = 5f;

    private Coroutine _hideCoroutine;
    private int _previewIndex = 0;


    private void Awake()
    {
        _slot = GetComponent<CookingSlot>();
        _highlightRenderer.enabled = false;
        _resultRenderer.enabled = false;
    }
    private void OnEnable()
    {
        _slot.OnStateChanged += HandleStateChanged;
        _slot.OnIngredientAdded += ShowIngredientPreview;
    }

    private void OnDisable()
    {
        _slot.OnStateChanged -= HandleStateChanged;
        _slot.OnIngredientAdded -= ShowIngredientPreview;
    }
    private void ShowIngredientPreview(IngredientSO ingredient)
    {
        _ingredientPreviews[_previewIndex].Play(ingredient.icon);
        _previewIndex = (_previewIndex + 1) % _ingredientPreviews.Length;
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
            CookingSlotState.Empty => _spriteEmpty,
            CookingSlotState.Filling => _spriteFilling,
            CookingSlotState.Cooking => _spriteCooking,
            CookingSlotState.Ready => _spriteReady,
            CookingSlotState.Spoiled => _spriteSpoiled,
            _ => _spriteEmpty,
        };
        if (state == CookingSlotState.Ready)
        {
            _resultRenderer.enabled = true;
            if (_slot.CookedRecipe != null)
            {
                _draggableFood.Setup(_slot.CookedRecipe, _slot);
                Sprite icon = _slot.CookedRecipe.icon;
                _resultRenderer.sprite = icon;
                //스프라이트 사이즈 조절 (icon.bounds.size는 스프라이트의 실제크기) 
                float maxDim = Mathf.Max(icon.bounds.size.x, icon.bounds.size.y);
                _resultRenderer.transform.localScale = Vector3.one * (_resultSize/maxDim);

            }
            else
            {
                _resultRenderer.sprite = _spriteFail;
                float maxDim = Mathf.Max(_spriteFail.bounds.size.x, _spriteFail.bounds.size.y);
                _resultRenderer.transform.localScale = Vector3.one * (_resultSize / maxDim);
            }
        }
        else
        {
            _resultRenderer.enabled = false;
        }
    }
    public void OnStartCookingButtonClicked()
    {
        float time = _testCookTime * UpgradeManager.instance.CookingSpeedMultiplier;
        _slot.StartCooking(time);
    }

}
