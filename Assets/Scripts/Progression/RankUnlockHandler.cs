using UnityEngine;

//TruckRankManager의 이벤트 구독
public class RankUnlockHandler : MonoBehaviour
{
    [SerializeField] private ContentRegistrySO _registry;

    private void Awake()
    {
        ApplyUnlocksUpToCurrentRank();
    }

    private void Start()
    {
        ApplyUnlocksUpToCurrentRank();

        if (TruckRankManager.instance != null)
            TruckRankManager.instance.OnRankUp += UnlockForRank;
    }
    private void OnDestroy()
    {
        if (TruckRankManager.instance != null)
        {
            TruckRankManager.instance.OnRankUp -= UnlockForRank;
        }
    }

    public void ApplyUnlocksUpToCurrentRank()
    {
        if (_registry == null || TruckRankManager.instance == null || UnlockManager.instance == null)
            return;

        for (int r = 1; r <= TruckRankManager.instance.CurrentRank; r++)
            UnlockForRank(r);
    }

    private void UnlockForRank(int rank)
    {
        if (_registry == null || UnlockManager.instance == null)
            return;

        foreach (var ghost in _registry.GetGhostsForRank(rank))
            UnlockManager.instance.UnlockGhost(ghost);

        foreach (var recipe in _registry.GetRecipesForRank(rank))
            UnlockManager.instance.UnlockRecipe(recipe);
    }

}
