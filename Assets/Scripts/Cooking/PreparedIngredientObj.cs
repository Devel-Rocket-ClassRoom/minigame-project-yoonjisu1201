using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class PreparedIngredientObj : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer _iconRenderer;

    private Collider2D _collider;
    private IngredientSO _ingredient;
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();

        Clear();
    }
    public void Setup(IngredientSO ingredient)
    {
        _ingredient = ingredient;
        _iconRenderer.sprite = ingredient.icon;
        _iconRenderer.enabled = true;

        _collider.enabled = true;
    }
    public void Clear()
    {
        _ingredient = null;

        if (_iconRenderer != null)
        {
            _iconRenderer.sprite = null;
            _iconRenderer.enabled = false;
        }

        if (_collider != null)
            _collider.enabled = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_ingredient == null)
            return;

        CookingSlotManager.Instance.AddIngredient(_ingredient);
    }
}
