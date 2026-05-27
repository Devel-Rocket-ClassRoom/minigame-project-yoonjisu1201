using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.AppUI.MVVM;
using UnityEngine;
//해금된 것들 저장, 해금 메서드, 해금 조회 메서드
public class UnlockManager : MonoBehaviour
{
    public static UnlockManager instance {  get; private set; }

    //----------------------------------해금 저장
    //HashSet - 중복 없이 저장, Contains 조회 빠름
    private HashSet<string> _unlockedRecipes = new();
    private HashSet<string> _unlockedGhosts = new();
    private HashSet<string> _unlockedIngredients = new();
    private HashSet<string> _unlockedArifacts = new();
    private HashSet<string> _unlockedAbilities = new(); //유물효과
    private HashSet<string> _unlockedMemoirIds = new(); //방명록

    private Dictionary<string, int> _artifactCounts = new();

    private const int ARTIFACT_ABILITY_THRESHOLD = 5;
    private int _artifactOverflowGold = 100;

    //----------------------------------해금 이벤트
    public event System.Action<GhostSO> OnGhostUnlocked;
    public event System.Action<RecipeSO> OnRecipeUnlocked;
    public event System.Action<IngredientSO> OnIngredientUnlocked;
    public event System.Action<ArtifactSO> OnArtifactUnlocked;
    public event System.Action<ArtifactSO> OnAbilityUnlocked;
    //public event System.Action<> OnMemoirUnlocked;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void UnlockGhost(GhostSO ghost)
    {
        if (_unlockedGhosts.Add(ghost.id))
        {
            Debug.Log($"유령 해금: {ghost.displayName}");
            OnGhostUnlocked?.Invoke(ghost);
        }
    }
    public void UnlockRecipe(RecipeSO recipe)
    {
        if (_unlockedRecipes.Add(recipe.id))
        {
            OnRecipeUnlocked?.Invoke (recipe);
            UnlockIngredientsFrom(recipe);
        }
    }
    public void UnlockIngredientsFrom(RecipeSO recipe)
    {
        foreach (var ing in recipe.basicIngredients)
            UnlockIngredient(ing);

        if (recipe.normalLast_Ing != null)
            UnlockIngredient(recipe.normalLast_Ing);

        if (recipe.special_Ingredient != null)
            UnlockIngredient(recipe.special_Ingredient);
    }
    public void UnlockIngredient(IngredientSO ing)
    {
        if (_unlockedIngredients.Add(ing.id))
        {
            OnIngredientUnlocked?.Invoke(ing);
            Debug.Log($"재료 해금: {ing.displayName}");
        }
    }
    //유물 개수, 해금만 관리. 
    public void CollectArtifact(ArtifactSO artifact)
    {
        if (!_artifactCounts.ContainsKey(artifact.id))
            _artifactCounts[artifact.id] = 0;

        int count = _artifactCounts[artifact.id];

        if (count >= ARTIFACT_ABILITY_THRESHOLD)
        {
            GoldManager.Instance.AddGold(_artifactOverflowGold);
            return;
        }

        _artifactCounts[artifact.id] = count + 1;

        if (count == 0)
        {
            _unlockedArifacts.Add(artifact.id);
            OnArtifactUnlocked?.Invoke(artifact);
            Debug.Log($"유물 해금: {IsArtifactUnlocked(artifact)}");
        }

        if (count + 1 == ARTIFACT_ABILITY_THRESHOLD)
        {
            _unlockedAbilities.Add(artifact.id);
            OnAbilityUnlocked?.Invoke(artifact);
            Debug.Log($"능력 해금: {IsAbilityUnlocked(artifact)}");
        }


    }

    //----------------------------------조회 메서드
    public bool IsGhostUnlocked(GhostSO ghost)
    {
        return ghost != null && _unlockedGhosts.Contains(ghost.id);
    }
    public bool IsRecipeUnlocked(RecipeSO recipe)
    {
        return recipe != null && _unlockedRecipes.Contains(recipe.id);
    }
    public bool IsIngredientUnlocked(IngredientSO ingredient)
    {
        return ingredient != null && _unlockedIngredients.Contains(ingredient.id);
    }
    public bool IsArtifactUnlocked(ArtifactSO artifact)
    {
        return artifact != null && _unlockedArifacts.Contains(artifact.id);
    }
    public bool IsAbilityUnlocked(ArtifactSO artifact)
    {
        return artifact != null && _unlockedAbilities.Contains(artifact.id);
    }

    public int GetArtifacCount(ArtifactSO artifact)
    {
        return artifact != null && 
            _artifactCounts.TryGetValue(artifact.id, out int c)  ? c : 0;
    }
}
