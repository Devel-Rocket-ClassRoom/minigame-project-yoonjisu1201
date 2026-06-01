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
    [SerializeField] private TextMeshPro _ingredientCountText;
    [SerializeField] private SpriteRenderer[] _ingredientIconSlots;

    //상태별 스프라이트 -> 나중에 애니메이션 교체
    [SerializeField] private Sprite _spriteEmpty;
    [SerializeField] private Sprite _spriteFilling;
    [SerializeField] private Sprite _spriteReady;
    [SerializeField] private Sprite _spriteSpoiled;
    [SerializeField] private Sprite _spriteFail;
    [SerializeField] private Sprite _spriteFailResult;

    [Header("애니메이션")]
    [SerializeField] private Animator _stateAnimator;

    public CookingSlot Slot => _slot;
    private float _testCookTime = 5f;

    private static readonly int StateParam = Animator.StringToHash("State");
    private Coroutine _hideCoroutine;
    private int _previewIndex = 0;


    private void Awake()
    {
        _slot = GetComponent<CookingSlot>();
        _highlightRenderer.enabled = false;
        _resultRenderer.enabled = false;
        if (_stateAnimator != null)
            _stateAnimator.enabled = false;
    }
    private void Start()
    {
        int level = UpgradeManager.instance.OrderBoardLevel;
        if (_ingredientCountText != null)
        {
            _ingredientCountText.sortingOrder = 20;
            if (level == 1)
                _ingredientCountText.text = "0/4";

            _ingredientCountText.enabled = level == 1;
        }
        if (_ingredientIconSlots != null)
            foreach (var slot in _ingredientIconSlots)
                slot.enabled = false;
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

        int level = UpgradeManager.instance.OrderBoardLevel;
        int count = _slot.Ingredients.Count;

        if (level == 1 && _ingredientCountText != null)
        {
            _ingredientCountText.text = $"{count}/4";
            _ingredientCountText.enabled = true;
        }
        if (level >= 2 && _ingredientIconSlots != null && count <= _ingredientIconSlots.Length)
        {
            _ingredientIconSlots[count - 1].sprite = ingredient.icon;
            _ingredientIconSlots[count - 1].enabled = true;
        }

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
        if (state == CookingSlotState.Empty)
        {
            if (_ingredientCountText != null)
                _ingredientCountText.text = "0/4";
            if (_ingredientIconSlots != null)
                foreach (var slot in _ingredientIconSlots)
                    slot.enabled = false;
            _draggableFood.enabled = false;
            _draggableFood.GetComponent<Collider2D>().enabled = false;
        }
        if (_ingredientCountText != null)
            _ingredientCountText.enabled = (state == CookingSlotState.Empty || state == CookingSlotState.Filling)
                && UpgradeManager.instance.OrderBoardLevel == 1;

        bool isCooking = state == CookingSlotState.Cooking;
        _stateAnimator.enabled = isCooking;
        if (isCooking)
        {
            _stateAnimator.SetInteger(StateParam, (int)state);
        }
        else
        {
            _stateRenderer.sprite = state switch
            {
                CookingSlotState.Empty => _spriteEmpty,
                CookingSlotState.Filling => _spriteFilling,
                CookingSlotState.Ready => _spriteReady,
                CookingSlotState.Spoiled => _spriteSpoiled,
                _ => _spriteEmpty,
            };
        }
        GetComponent<Collider2D>().enabled = state != CookingSlotState.Ready;
        if (state == CookingSlotState.Ready)
        {
            if (_slot.CookedRecipe != null)
            {
                _draggableFood.Setup(_slot.CookedRecipe, _slot);
                Sprite icon = _slot.CookedRecipe.icon;
                if (icon != null)
                {
                    _resultRenderer.enabled = true;
                    _resultRenderer.sprite = icon;
                }
            }
            else
            {
                _draggableFood.Setup(null, _slot);
                if (_spriteFail != null)
                    _stateRenderer.sprite = _spriteFail;
                if (_spriteFailResult != null)
                {
                    _resultRenderer.enabled = true;
                    _resultRenderer.sprite = _spriteFailResult;
                }
                else
                {
                    _resultRenderer.enabled = false;
                }
            }
        }
        else
        {
            _resultRenderer.enabled = false;
        }

    }
    public void StartCooking()
    {
        float time = _testCookTime * UpgradeManager.instance.CookingSpeedMultiplier;
        _slot.StartCooking(time);
    }

}
