using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//선택창 열기, 레시피 목록 생성, 최대선택 개수 제한, 선택완료 시 선택한 레시피리스트 전달
public class PreparedRecipeSelectUI : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject _panel;

    [Header("왼쪽 레시피 목록")]
    [SerializeField] private Transform _recipeButtonRoot;
    [SerializeField] private PreparedRecipeSelectButtonUI _recipeButtonPrefab;

    [Header("오른쪽 선택 슬롯 1")]
    [SerializeField] private Image _selectedRecipeIcon1;
    [SerializeField] private TMP_Text _selectedRecipeName1;
    [SerializeField] private Image _slot1AddIcon;
    [SerializeField] private Image _slot1BasicIcon1;
    [SerializeField] private Image _slot1BasicIcon2;
    [SerializeField] private Image _slot1BasicIcon3;
    [SerializeField] private Image _slot1NormalLastIcon;
    [SerializeField] private Image _slot1SpecialLastIcon;

    [Header("오른쪽 선택 슬롯 2")]
    [SerializeField] private Image _selectedRecipeIcon2;
    [SerializeField] private TMP_Text _selectedRecipeName2;
    [SerializeField] private Image _slot2AddIcon;
    [SerializeField] private Image _slot2BasicIcon1;
    [SerializeField] private Image _slot2BasicIcon2;
    [SerializeField] private Image _slot2BasicIcon3;
    [SerializeField] private Image _slot2NormalLastIcon;
    [SerializeField] private Image _slot2SpecialLastIcon;

    [Header("슬롯2 잠금")]
    [SerializeField] private TMP_Text _slot2LockedText;

    [Header("하단")]
    [SerializeField] private TMP_Text _selectedCountText;
    [SerializeField] private Button _confirmButton;

    private readonly List<RecipeSO> _selectedRecipes = new();
    private readonly List<PreparedRecipeSelectButtonUI> _createdButtons = new();

    private int _maxSelectCount;
    private Action<List<RecipeSO>> _onConfirm;
    private void Awake()
    {
        _confirmButton.onClick.AddListener(Confirm);
        _panel.SetActive(false);
    }
    public void Open(
        List<RecipeSO> availableRecipes,
        int maxSelectCount,
        Action<List<RecipeSO>> onConfirm)
    {
        _maxSelectCount = maxSelectCount;
        _onConfirm = onConfirm;

        _selectedRecipes.Clear();

        bool slot2Locked = maxSelectCount < 2;
        if (_slot2LockedText != null)
            _slot2LockedText.gameObject.SetActive(slot2Locked);
        if (_selectedRecipeName2 != null)
            _selectedRecipeName2.gameObject.SetActive(!slot2Locked);
        if (_slot2AddIcon != null)
            _slot2AddIcon.gameObject.SetActive(!slot2Locked);

        ClearRecipeButtons();
        CreateRecipeButtons(availableRecipes);
        RefreshSelectedSlotUI();

        _panel.SetActive(true);
    }
    private void CreateRecipeButtons(List<RecipeSO> availableRecipes)
    {
        foreach (var recipe in availableRecipes)
        {
            var button = Instantiate(_recipeButtonPrefab, _recipeButtonRoot);
            button.Setup(recipe, OnRecipeButtonClicked);

            _createdButtons.Add(button);
        }
    }
    private void OnRecipeButtonClicked(
        PreparedRecipeSelectButtonUI clickedButton,
        RecipeSO recipe)
    {
        if (_selectedRecipes.Contains(recipe))
        {
            _selectedRecipes.Remove(recipe);
            clickedButton.SetSelected(false);

            RefreshSelectedSlotUI();
            return;
        }

        if (_selectedRecipes.Count >= _maxSelectCount)
        {
            Debug.Log("준비 레시피 슬롯이 가득 찼습니다.");
            return;
        }

        _selectedRecipes.Add(recipe);
        clickedButton.SetSelected(true);

        RefreshSelectedSlotUI();
    }
    private void RefreshSelectedSlotUI()
    {
        SetSelectedSlot(
            _selectedRecipeIcon1, _selectedRecipeName1, _slot1AddIcon,
            _slot1BasicIcon1, _slot1BasicIcon2, _slot1BasicIcon3,
            _slot1NormalLastIcon, _slot1SpecialLastIcon,
            _selectedRecipes.Count > 0 ? _selectedRecipes[0] : null);

        SetSelectedSlot(
            _selectedRecipeIcon2, _selectedRecipeName2, _slot2AddIcon,
            _slot2BasicIcon1, _slot2BasicIcon2, _slot2BasicIcon3,
            _slot2NormalLastIcon, _slot2SpecialLastIcon,
            _selectedRecipes.Count > 1 ? _selectedRecipes[1] : null);

        if (_selectedCountText != null)
            _selectedCountText.text = $"선택 {_selectedRecipes.Count} / {_maxSelectCount}";
    }
    private void SetSelectedSlot(
        Image iconImage, TMP_Text nameText, Image addIcon,
        Image basicIcon1, Image basicIcon2, Image basicIcon3,
        Image normalLastIcon, Image specialLastIcon,
        RecipeSO recipe)
    {
        if (recipe == null)
        {
            if (iconImage != null) { iconImage.sprite = null; iconImage.enabled = false; }
            if (nameText != null) nameText.text = "레시피를 선택해주세요";
            if (addIcon != null) addIcon.enabled = true;
            SetIngredientIcon(basicIcon1, null);
            SetIngredientIcon(basicIcon2, null);
            SetIngredientIcon(basicIcon3, null);
            SetIngredientIcon(normalLastIcon, null);
            SetIngredientIcon(specialLastIcon, null);
            return;
        }

        if (iconImage != null) 
        {
            if (!recipe.isSignatureMenu)
                iconImage.sprite = recipe.icon;
            else
                iconImage.sprite = recipe.specialIcon;

            iconImage.enabled = true; 
        }
        if (nameText != null) nameText.text = LocalizationManager.GetRecipeName(recipe.id);
        if (addIcon != null) addIcon.enabled = false;

        SetIngredientIcon(basicIcon1, recipe.basicIngredients.Count > 0 ? recipe.basicIngredients[0] : null);
        SetIngredientIcon(basicIcon2, recipe.basicIngredients.Count > 1 ? recipe.basicIngredients[1] : null);
        SetIngredientIcon(basicIcon3, recipe.basicIngredients.Count > 2 ? recipe.basicIngredients[2] : null);
        SetIngredientIcon(normalLastIcon, recipe.normalLast_Ing);
        SetIngredientIcon(specialLastIcon, recipe.special_Ingredient);
    }
    private void SetIngredientIcon(Image image, IngredientSO ingredient)
    {
        if (image == null) return;

        if (ingredient == null)
        {
            image.sprite = null;
            image.enabled = false;
            return;
        }
        image.sprite = ingredient.icon;
        image.enabled = true;
    }
    private void Confirm()
    {
        _panel.SetActive(false);

        _onConfirm?.Invoke(new List<RecipeSO>(_selectedRecipes));
    }
    private void ClearRecipeButtons()
    {
        for (int i = _recipeButtonRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(_recipeButtonRoot.GetChild(i).gameObject);
        }

        _createdButtons.Clear();
    }
}
