using System.Collections.Generic;
using UnityEngine;

public class PreparedRecipeManager : MonoBehaviour
{
    public static PreparedRecipeManager Instance { get; private set; }

    [Header("선택창 UI")]
    [SerializeField] private PreparedRecipeSelectUI _selectUI;

    [Header("영업 중 보이는 월드 슬롯")]
    [SerializeField] private PreparedRecipeWorldSlot[] _worldSlots;

    [SerializeField] private ContentRegistrySO _registry;


    private void Awake()
    {
        Instance = this;
    }

    public void BeginPrepareFlow()
    {
        ClearAllWorldSlots();

        int slotCount = UpgradeManager.instance.ContainerSlotCount;
        if (slotCount <= 0)
        {
            SessionManager.instance.StartSessionAfterPrepare();
            return;
        }

        if (_selectUI == null)
        {
            SessionManager.instance.StartSessionAfterPrepare();
            return;
        }

        if (_registry == null)
        {
            SessionManager.instance.StartSessionAfterPrepare();
            return;
        }
        _selectUI.Open(
            UnlockManager.instance.GetUnlockedRecipes(_registry),
            slotCount,
            OnRecipeSelectionConfirmed
        );
    }
    private void OnRecipeSelectionConfirmed(List<RecipeSO> selectedRecipes)
    {
        ApplyPreparedRecipes(selectedRecipes);

        SessionManager.instance.StartSessionAfterPrepare();
    }

    public void ApplyPreparedRecipes(List<RecipeSO> selectedRecipes)
    {
        ClearAllWorldSlots();

        int count = Mathf.Min(selectedRecipes.Count, _worldSlots.Length);

        for (int i = 0; i < count; i++)
        {
            _worldSlots[i].Setup(selectedRecipes[i]);
        }
    }
    private void ClearAllWorldSlots()
    {
        foreach (var slot in _worldSlots)
        {
            slot.Clear();
        }
    }

}
