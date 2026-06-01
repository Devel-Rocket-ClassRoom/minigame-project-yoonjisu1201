using TMPro;
using UnityEngine;

public class PreparedRecipeWorldSlot : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private SpriteRenderer _recipeIcon;

    [Header("재료 버튼 5개")]
    [SerializeField] private PreparedIngredientObj _basicButton1;
    [SerializeField] private PreparedIngredientObj _basicButton2;
    [SerializeField] private PreparedIngredientObj _basicButton3;
    [SerializeField] private PreparedIngredientObj _normalLastButton;
    [SerializeField] private PreparedIngredientObj _specialLastButton;

    public void Setup(RecipeSO recipe)
    {
        if (recipe == null)
        {
            Clear();
            return;
        }

        _root.SetActive(true);

        _recipeIcon.sprite = recipe.isSignatureMenu ? recipe.specialIcon : recipe.icon;
        _recipeIcon.enabled = true;
        ClearIngredientButtons();

        if (recipe.basicIngredients.Count > 0)
            _basicButton1.Setup(recipe.basicIngredients[0]);

        if (recipe.basicIngredients.Count > 1)
            _basicButton2.Setup(recipe.basicIngredients[1]);

        if (recipe.basicIngredients.Count > 2)
            _basicButton3.Setup(recipe.basicIngredients[2]);

        if (recipe.normalLast_Ing != null)
            _normalLastButton.Setup(recipe.normalLast_Ing);

        if (recipe.special_Ingredient != null)
            _specialLastButton.Setup(recipe.special_Ingredient);

    }
    public void Clear()
    {
        if (_recipeIcon != null)
        {
            _recipeIcon.sprite = null;
            _recipeIcon.enabled = false;
        }

        ClearIngredientButtons();

        _root.SetActive(false);
    }

    private void ClearIngredientButtons()
    {
        _basicButton1.Clear();
        _basicButton2.Clear();
        _basicButton3.Clear();
        _normalLastButton.Clear();
        _specialLastButton.Clear();
    }
}
