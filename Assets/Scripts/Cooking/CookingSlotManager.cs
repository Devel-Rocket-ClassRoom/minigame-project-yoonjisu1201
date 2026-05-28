using System.Collections.Generic;
using UnityEngine;

//조리대 슬롯 전체를 관리하는 매니저. 활성슬롯 전환
public class CookingSlotManager : MonoBehaviour
{
    public static CookingSlotManager Instance {  get; private set; }

    [SerializeField] private List<CookingSlotUI> _slots; 
    private CookingSlotUI _activeSlotUI;

    //현재 활성화된 슬롯이 있으면 슬롯반환
    public CookingSlot ActiveSlot => _activeSlotUI?.Slot;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (_slots.Count > 0)
        {
            SetActiveSlot(_slots[0]);
        }
        ApplySlotUpgrade();

    }
  
    public void SetActiveSlot(CookingSlotUI slotUI)
    {
        _activeSlotUI?.SetHighlight(false);
        _activeSlotUI = slotUI;
        _activeSlotUI?.SetHighlight(true);
    }
    public void AddIngredient(IngredientSO ingredient)
    {
        ActiveSlot?.AddIngredient(ingredient);
    }
    public void OnCancelCooking()
    {
        ActiveSlot?.CancelCooking();
    }
    public void ApplySlotUpgrade()
    {
        int active = UpgradeManager.instance.ActiveSlotCount;
        for (int i = 0; i < _slots.Count; i++)
            _slots[i].gameObject.SetActive(i < active);

        if (!_activeSlotUI.gameObject.activeSelf)
            SetActiveSlot(_slots[0]);
    }

}
