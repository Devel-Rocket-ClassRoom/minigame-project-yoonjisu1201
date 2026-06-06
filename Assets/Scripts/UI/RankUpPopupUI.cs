using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUpPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private ContentRegistrySO _registry;

    [SerializeField] private RectTransform _cardContainer;
    [SerializeField] private GameObject _cardPrefab;

    private void Start()
    {
        _panel.SetActive(false);
        _closeButton.onClick.AddListener(Close);

        if (TruckRankManager.instance == null || _registry == null) return;
        int rank = TruckRankManager.instance.PendingRankUp;
        if (rank <= 0) return;

        BuildCards(rank);
        _panel.SetActive(true);
    }

    private void BuildCards(int rank)
{
        _titleText.text = $"트럭등급 {rank} 달성!";

        foreach (var ghost in _registry.GetGhostsForRank(rank))
        {
            PlayerPrefs.SetInt("new_ghost_" + ghost.id, 1);
            AddCard(ghost.icon, LocalizationManager.GetGhostName(ghost.id), "손님");
        }

        var recipes = _registry.GetRecipesForRank(rank);

        foreach (var recipe in recipes)
            if (!recipe.isSignatureMenu)
            {
                PlayerPrefs.SetInt("new_recipe_" + recipe.id, 1);
                AddCard(recipe.icon, LocalizationManager.GetRecipeName(recipe.id), "일반레시피");
            }

        foreach (var recipe in recipes)
            if (recipe.isSignatureMenu)
            {
                PlayerPrefs.SetInt("new_recipe_" + recipe.id, 1);
                AddCard(recipe.icon, LocalizationManager.GetRecipeName(recipe.id), "일반레시피");
                AddCard(recipe.specialIcon, LocalizationManager.GetRecipeName(recipe.id), "전용레시피");
            }

        //재료
        var previousIngredients = new HashSet<string>();
        for (int r = 1; r < rank; r++)
        {
            foreach (var prev in _registry.GetRecipesForRank(r))
            {
                foreach (var ing in prev.basicIngredients)
                    if (ing != null) previousIngredients.Add(ing.id);
                if (prev.normalLast_Ing != null) previousIngredients.Add(prev.normalLast_Ing.id);
                if (prev.special_Ingredient != null) previousIngredients.Add(prev.special_Ingredient.id);
            }
        }

        var shown = new HashSet<string>();
        foreach (var recipe in recipes)
        {
            TryAddIngredient(recipe.normalLast_Ing, previousIngredients, shown, "일반재료");
            foreach (var ing in recipe.basicIngredients)
                TryAddIngredient(ing, previousIngredients, shown, "일반재료");
        }
        foreach (var recipe in recipes)
            TryAddIngredient(recipe.special_Ingredient, previousIngredients, shown, "전용재료");
    }
    private void TryAddIngredient(IngredientSO ing, HashSet<string> previous, HashSet<string> shown, string category)
    {
        if (ing != null && !previous.Contains(ing.id) && shown.Add(ing.id))
            AddCard(ing.icon, LocalizationManager.GetIngredientName(ing.id), category);
    }
    private void AddCard(Sprite icon, string name, string category)
    {
        var card = Instantiate(_cardPrefab, _cardContainer).GetComponent<RankUpCardUI>();
        card.Setup(icon, name, category);
    }
    public event System.Action OnClosed;

    private void Close()
    {
        TruckRankManager.instance.ClearPendingRankUp();
        _panel.SetActive(false);
        OnClosed?.Invoke();
    }

    [ContextMenu("Test/Show Level Up Popup")]
    public void Test_ShowPopup()
    {
        BuildCards(1);
        _panel.SetActive(true);
    }
}
