using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//전체 레시피, 유령 보유
//현재 레벨에 해당하는 레시피, 유령 리스트 반환메서드
[CreateAssetMenu(fileName = "SO_ContentRegistry", menuName = "Gmae/Content Registry")]
public class ContentRegistrySO : ScriptableObject
{
    public List<GhostSO> allGhosts;
    public List<RecipeSO> allRecipes;

    public List<GhostSO> GetGhostsForRank(int rank)
    {
        var result = new List<GhostSO>();
        foreach (var ghost in allGhosts)
        {
            if (ghost.unlockRank == rank)
                result.Add(ghost);
        }
        return result;
    }
    public List<RecipeSO> GetRecipesForRank(int rank)
    {
        var result = new List<RecipeSO>();
        foreach (var recipe in allRecipes)
        {
            if (recipe.unlockRank == rank)
                result.Add(recipe);
        }
        return result;
    }
}
