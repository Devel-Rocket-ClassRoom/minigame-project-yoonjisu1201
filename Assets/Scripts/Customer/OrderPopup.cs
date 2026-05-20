using UnityEngine;

public class OrderPopup : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _orderIcon;
    [SerializeField] private SpriteRenderer _menuIcon;

     public void Show(RecipeSO recipe)
    {
        if (recipe == null) return;

        _menuIcon.sprite = recipe.icon;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
