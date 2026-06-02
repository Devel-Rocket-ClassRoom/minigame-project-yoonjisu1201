using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientObject : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private IngredientSO _ingredient;

    public IngredientSO Ingredient => _ingredient;
    private void Start()
    {
        if (!UnlockManager.instance.IsIngredientUnlocked(_ingredient))
            gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (CookingGuideManager.instance != null &&
        !CookingGuideManager.instance.IsIngredientAllowed(_ingredient))
            return;

        CookingSlotManager.Instance.AddIngredient(_ingredient);
    }
}
