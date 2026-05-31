using UnityEngine;

//영업씬 - 재료보관슬롯 관리
public class RecipeContainerManager : MonoBehaviour
{
    public static RecipeContainerManager instance { get; private set; }

    private readonly RecipeSO[] _containers = new RecipeSO[2];

    public int MaxSlots => UpgradeManager.instance != null ? 
        UpgradeManager.instance.ContainerSlotCount : 0;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public RecipeSO GetContainer(int index) =>
        (index >= 0 && index < 2) ? _containers[index] : null;

    public bool IsLocked { get; private set; }

    public void Unlock() => IsLocked = false;
    public void Lock() => IsLocked = true;

    public bool SetContainer(int index, RecipeSO recipe)
    {
        if (IsLocked) return false;
        if (index < 0 || index >= MaxSlots) return false;
        if (recipe != null && !UnlockManager.instance.IsRecipeUnlocked(recipe)) return false;
        _containers[index] = recipe;
        return true;
    }
    public void ClearContainer(int index)
    {
        if (IsLocked) return;
        if (index >= 0 && index < 2) _containers[index] = null;
    }
}
