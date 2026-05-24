using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//재료 탭버튼 클릭 - 재료 버튼들 생성 - 재료 버튼 클릭
public class IngredientPanelUI : MonoBehaviour
{
    [SerializeField] private Button _tabBase;
    [SerializeField] private Button _tabFresh;
    [SerializeField] private Button _tabSauce;
    [SerializeField] private Button _tabOther;
    [SerializeField] private Button _tabSpecial;
    [SerializeField] private Transform _buttonContainer;

    [SerializeField] private GameObject _ingredientPrefab;
    [SerializeField] private List<IngredientSO> _allIngredients;

    private List<IngredientSO> _sessionIngredients = new List<IngredientSO>();
    private IngredientCategory _currentCategory;

    public void Start()
    {
        foreach (var ingredient in _allIngredients)
        {
            if (UnlockManager.instance.IsIngredientUnlocked(ingredient))
                _sessionIngredients.Add(ingredient);
        }

        _currentCategory = IngredientCategory.Base;
        ShowCategory(IngredientCategory.Base);
        //나중에 깔끔하게 바꾸기
        _tabBase.onClick.AddListener(() => ShowCategory(IngredientCategory.Base));
        _tabFresh.onClick.AddListener(() => ShowCategory(IngredientCategory.Fresh));
        _tabSauce.onClick.AddListener(() => ShowCategory(IngredientCategory.Sauce));
        _tabOther.onClick.AddListener(() => ShowCategory(IngredientCategory.Other));
        _tabSpecial.onClick.AddListener(() => ShowCategory(IngredientCategory.Special));
    }

    private void ShowCategory(IngredientCategory category)
    {
        //카테고리 눌렀을때 버튼들만 생성 -> 버튼 정보 세팅은  재료버튼UI에서 메서드 참조
        _currentCategory = category;
        foreach (Transform child in _buttonContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var Ingredient in _sessionIngredients)
        {
            if (Ingredient.category != category)
                continue;

            var buttonObj = Instantiate(_ingredientPrefab, _buttonContainer);
            IngredientButtonUI buttonUI = buttonObj.GetComponent<IngredientButtonUI>();
            buttonUI.Setup(Ingredient, OnIngredientButtonClicked);
        }
    }
    private void OnIngredientButtonClicked(IngredientSO ingredient)
    {
        //재료 눌렀을때 조리대에 재료 추가
        CookingSlotManager.Instance.AddIngredient(ingredient);
        //Debug.Log($"{ingredient.displayName} 선택");
    }
}
