using UnityEngine;
using UnityEngine.EventSystems;

public class CookButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _spriteNormal;
    [SerializeField] private Sprite _spriteReady;
    [SerializeField] private CookingSlot _slot;

    private void OnEnable()
    {
        _renderer.sprite = _spriteNormal;
        _slot.OnIngredientsFull += OnSlotFull;
        _slot.OnStateChanged += OnSlotStateChanged;
    }

    private void OnDisable()
    {
        _slot.OnIngredientsFull -= OnSlotFull;
        _slot.OnStateChanged -= OnSlotStateChanged;
    }

    private void OnSlotFull()
    {
        _renderer.sprite = _spriteReady;
    }

    private void OnSlotStateChanged(CookingSlotState state)
    {
        if (state != CookingSlotState.Filling)
            _renderer.sprite = _spriteNormal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CookingGuideManager.instance != null &&
        (CookingGuideManager.instance.StepIndex == 1 ||
        (CookingGuideManager.instance.StepIndex == 4 && CookingGuideManager.instance.SubStep < 2)))
            return;

        CookingSlotManager.Instance.OnStartCooking();
    }
}
