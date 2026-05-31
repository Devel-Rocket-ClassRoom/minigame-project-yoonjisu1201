using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreparedRecipeSelectButtonUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button _button;

    [Header("기본 표시")]
    [SerializeField] private Image _recipeIconImage;
    [SerializeField] private TMP_Text _recipeNameText;

    //[Header("재료 아이콘 5개")]
    //[SerializeField] private Image _basicIcon1;
    //[SerializeField] private Image _basicIcon2;
    //[SerializeField] private Image _basicIcon3;
    //[SerializeField] private Image _normalLastIcon;
    //[SerializeField] private Image _specialLastIcon;

    [Header("선택 표시")]
    [SerializeField] private GameObject _checkMark;
    [SerializeField] private Image _selectedFrame;

    private RecipeSO _recipe;
    private Action<PreparedRecipeSelectButtonUI, RecipeSO> _onClicked;
    private bool _isSelected;
    public RecipeSO Recipe => _recipe;
    public bool IsSelected => _isSelected;
    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        _button.transition = Selectable.Transition.None;
        _button.onClick.AddListener(OnClicked);
    }


    public void Setup(
        RecipeSO recipe,
        Action<PreparedRecipeSelectButtonUI, RecipeSO> onClicked)
    {
        _recipe = recipe;
        _onClicked = onClicked;

        if (!_recipe.isSignatureMenu)
            _recipeIconImage.sprite = recipe.icon;
        else
            _recipeIconImage.sprite = _recipe.specialIcon;

        _recipeNameText.text = LocalizationManager.GetRecipeName(recipe.id);

        //SetIngredientIcon(_basicIcon1, recipe.basicIngredients.Count > 0 ? recipe.basicIngredients[0] : null);
        //SetIngredientIcon(_basicIcon2, recipe.basicIngredients.Count > 1 ? recipe.basicIngredients[1] : null);
        //SetIngredientIcon(_basicIcon3, recipe.basicIngredients.Count > 2 ? recipe.basicIngredients[2] : null);
        //SetIngredientIcon(_normalLastIcon, recipe.normalLast_Ing);
        //SetIngredientIcon(_specialLastIcon, recipe.special_Ingredient);

        SetSelected(false);
    }
    public void OnClicked()
    {
        if (_recipe == null)
            return;

        _onClicked?.Invoke(this, _recipe);
    }
    public void SetSelected(bool selected)
    {
        _isSelected = selected;

        if (_checkMark != null)
            _checkMark.SetActive(selected);

        if (_selectedFrame != null)
            _selectedFrame.enabled = selected;
    }
    private void SetIngredientIcon(Image image, IngredientSO ingredient)
    {
        if (image == null)
            return;

        if (ingredient == null)
        {
            image.sprite = null;
            image.enabled = false;
            return;
        }

        image.sprite = ingredient.icon;
        image.enabled = true;
    }
}
