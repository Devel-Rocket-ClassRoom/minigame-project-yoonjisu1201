using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientObject : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private IngredientSO _ingredient;
    private void Start()
    {
        if (!UnlockManager.instance.IsIngredientUnlocked(_ingredient))
            gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        CookingSlotManager.Instance.AddIngredient(_ingredient);
    }
}
