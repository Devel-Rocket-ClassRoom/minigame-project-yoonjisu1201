using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

//레시피 카드 1장 담당
public class RecipeCollectionCardUI : MonoBehaviour
{
    [SerializeField] private Image _recipeImage;
    [SerializeField] private TextMeshProUGUI _recipeNameText;
    [SerializeField] private GameObject _newBadge;

    private static readonly Color LOCKED_COLOR = Color.black;
    private static readonly Color UNLOCKED_COLOR = Color.white;

    private RecipeSO _recipeData;
    private Button _button;
    private System.Action<RecipeSO> _onSelected;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }
    public void Setup(RecipeSO recipe, System.Action<RecipeSO> onSelected)
    {
        _recipeData = recipe;
        _onSelected = onSelected;
        Refresh();
    }
    private void Refresh()
    {
        if (_recipeData == null) return;

        bool unlocked = UnlockManager.instance != null && UnlockManager.instance.IsRecipeUnlocked(_recipeData);

        _recipeImage.sprite = _recipeData.isSignatureMenu ? _recipeData.specialIcon : _recipeData.icon;
        _recipeImage.color = unlocked ? UNLOCKED_COLOR : LOCKED_COLOR;
        _recipeNameText.text = unlocked ?
            LocalizationManager.GetRecipeName(_recipeData.id) : "???";
        _button.interactable = true;
        if (_newBadge != null)
            _newBadge.SetActive(unlocked && PlayerPrefs.GetInt("new_recipe_" + _recipeData.id, 0) == 1);
    }

    private void OnClicked()
    {
        if (_recipeData == null) return;
        PlayerPrefs.DeleteKey("new_recipe_" + _recipeData.id);
        if (_newBadge != null) _newBadge.SetActive(false);
        _onSelected?.Invoke(_recipeData);
    }
}
