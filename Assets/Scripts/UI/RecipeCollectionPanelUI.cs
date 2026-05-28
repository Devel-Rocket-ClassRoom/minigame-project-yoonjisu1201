using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 도감 레시피탭 - 카드 목록 + 페이지네이션 관리
public class RecipeCollectionPanelUI : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;
    [SerializeField] private RecipeCollectionCardUI[] _cards;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TextMeshProUGUI _pageText;
    [SerializeField] private RecipeDetailPanelUI _detailPanel;

    private int _currentPage = 0;
    private int TotalPages => Mathf.CeilToInt((float)_registry.allRecipes.Count / _cards.Length);

    private void Start()
    {
        _leftButton.onClick.AddListener(PrevPage);
        _rightButton.onClick.AddListener(NextPage);

        if (UnlockManager.instance != null)
            UnlockManager.instance.OnRecipeUnlocked += _ => RefreshPage();

        RefreshPage();

        if (_registry.allRecipes.Count > 0)
            _detailPanel.ShowRecipe(_registry.allRecipes[0]);
    }

    private void RefreshPage()
    {
        int startIndex = _currentPage * _cards.Length;

        for (int i = 0; i < _cards.Length; i++)
        {
            int recipeIndex = startIndex + i;
            if (recipeIndex < _registry.allRecipes.Count)
            {
                _cards[i].gameObject.SetActive(true);
                _cards[i].Setup(_registry.allRecipes[recipeIndex], OnCardSelected);
            }
            else
                _cards[i].gameObject.SetActive(false);
        }

        _pageText.text = $"{_currentPage + 1} / {TotalPages}";
        _leftButton.interactable = _currentPage > 0;
        _rightButton.interactable = _currentPage < TotalPages - 1;
    }

    private void OnCardSelected(RecipeSO recipe)
    {
        _detailPanel.ShowRecipe(recipe);
    }

    private void PrevPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            RefreshPage();
        }
    }

    private void NextPage()
    {
        if (_currentPage < TotalPages - 1)
        {
            _currentPage++;
            RefreshPage();
        }
    }
}
