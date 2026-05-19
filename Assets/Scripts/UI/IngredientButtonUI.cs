using UnityEngine;
using UnityEngine.UI;

public class IngredientButtonUI : MonoBehaviour
{
    [SerializeField] private Image _icon;

    private System.Action<IngredientSO> _onClicked;
    private IngredientSO _ingredient;

    public void Setup(IngredientSO ingredient, System.Action<IngredientSO> onClicked)
    {
        _ingredient = ingredient;
        _icon.sprite = ingredient.icon;
        _onClicked = onClicked;
        GetComponent<Button>().onClick.AddListener(() => _onClicked?.Invoke(_ingredient));
    }
}
