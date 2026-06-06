using System.Collections.Generic;
using System.Data;
using UnityEngine;
//해금된 것들 저장, 해금 메서드, 해금 조회 메서드
public class UnlockManager : MonoBehaviour
{
    public static UnlockManager instance { get; private set; }

    //----------------------------------해금 저장
    //HashSet - 중복 없이 저장, Contains 조회 빠름
    private HashSet<string> _unlockedRecipes = new();
    private HashSet<string> _unlockedGhosts = new();
    private HashSet<string> _unlockedIngredients = new();
    private HashSet<string> _unlockedArifacts = new();
    private HashSet<string> _unlockedAbilities = new(); //유물효과
    private HashSet<string> _unlockedMemoirIds = new(); //방명록

    private Dictionary<string, int> _artifactCounts = new();
    private List<ArtifactSO> _sessionArtifacts = new();

    [Header("DEBUG")]
    [SerializeField] private int _debugArtifactCount = 0;
    [SerializeField] private int _debugMaxGhostRank = 1;  // 추가
    [SerializeField] private ContentRegistrySO _debugRegistry;

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

#if UNITY_EDITOR
        if (_debugArtifactCount > 0 && _debugRegistry != null)
            foreach (var ghost in _debugRegistry.allGhosts)
            {
                if (ghost.artifact == null) continue;
                if (ghost.unlockRank > _debugMaxGhostRank) continue;  // 랭크 초과 스킵
                for (int i = 0; i < _debugArtifactCount; i++)
                    CollectArtifact(ghost.artifact);
            }
#endif
    }
    public void UnlockGhost(GhostSO ghost)
    {
        if (_unlockedGhosts.Add(ghost.id))
        {
            OnGhostUnlocked?.Invoke(ghost);
        }
    }
    public void UnlockRecipe(RecipeSO recipe)
    {
        if (_unlockedRecipes.Add(recipe.id))
        {
            OnRecipeUnlocked?.Invoke(recipe);
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
        }
    }
    public void GatherArtifact(ArtifactSO artifact)
    {
        _sessionArtifacts.Add(artifact);
    }

    public void CommitSessionArtifacts()
    {
        foreach (var artifact in _sessionArtifacts)
            CollectArtifact(artifact);
        _sessionArtifacts.Clear();
    }

    public void ResetSessionArtifacts()
    {
        _sessionArtifacts.Clear();
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
            PlayerPrefs.SetInt("new_artifact_" + artifact.id, 1);
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
            _artifactCounts.TryGetValue(artifact.id, out int c) ? c : 0;
    }
    public List<RecipeSO> GetUnlockedRecipes(ContentRegistrySO registry)
    {
        var result = new List<RecipeSO>();
        foreach (var recipe in registry.allRecipes)
            if (IsRecipeUnlocked(recipe)) result.Add(recipe);
        return result;
    }

    //----------------------------------저장/불러기용
    public void WriteTo(SaveData data)
    {
        data.unlockedRecipes = new List<string>(_unlockedRecipes);
        data.unlockedGhosts = new List<string>(_unlockedGhosts);
        data.unlockedIngredients = new List<string>(_unlockedIngredients);
        data.unlockedArtifacts = new List<string>(_unlockedArifacts);
        data.unlockedAbilities = new List<string>(_unlockedAbilities);

        //타입이 리스트라서 save를 연속으로 하면 중복으로 저장되기때문에 clear
        data.artifactCountKeys.Clear();
        data.artifactCountValues.Clear();
        foreach (var kv in _artifactCounts)
        {
            data.artifactCountKeys.Add(kv.Key);
            data.artifactCountValues.Add(kv.Value);
        }
    }

    public void LoadFrom(SaveData data)
    {
        _unlockedRecipes = new HashSet<string>(data.unlockedRecipes);
        _unlockedGhosts = new HashSet<string>(data.unlockedGhosts);
        _unlockedIngredients = new HashSet<string>(data.unlockedIngredients);
        _unlockedArifacts = new HashSet<string>(data.unlockedArtifacts);
        _unlockedAbilities = new HashSet<string>(data.unlockedAbilities);

        for (int i = 0; i < data.artifactCountKeys.Count; i++)
            _artifactCounts[data.artifactCountKeys[i]] = data.artifactCountValues[i];
    }

    public void ResetAll()
    {
        _unlockedRecipes.Clear();
        _unlockedGhosts.Clear();
        _unlockedIngredients.Clear();
        _unlockedArifacts.Clear();
        _unlockedAbilities.Clear();
        _unlockedMemoirIds.Clear();
        _artifactCounts.Clear();
    }
}
