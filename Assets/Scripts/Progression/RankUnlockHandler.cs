using UnityEngine;

//TruckRankManager의 이벤트 구독
public class RankUnlockHandler : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;

    private void Awake()
    {
        if (true)
        {
            UnlockForRank(1);
        }
    }

    private void Start()
    {
        TruckRankManager.instance.OnRankUp += UnlockForRank;
    }
    private void OnDestroy()
    {
        if (TruckRankManager.instance != null)
        {
            TruckRankManager.instance.OnRankUp -= UnlockForRank;
        }
    }
    private void UnlockForRank(int rank)
    {
        foreach (var ghost in _registry.GetGhostsForRank(rank))
            UnlockManager.instance.UnlockGhost(ghost);

        foreach (var recipe in _registry.GetRecipesForRank(rank))
            UnlockManager.instance.UnlockRecipe(recipe);
    }

}
