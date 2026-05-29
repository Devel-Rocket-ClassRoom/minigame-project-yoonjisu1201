using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class UpgradeSectionUI
{
    public Button button;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
}
public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] private UpgradeSectionUI _slot;
    [SerializeField] private UpgradeSectionUI _speed;
    [SerializeField] private UpgradeSectionUI _orderBoard;
    [SerializeField] private UpgradeSectionUI _orderHint;
    [SerializeField] private UpgradeSectionUI _favorite;
    private void OnEnable()
    {
        Refresh();
    }
    private void Start()
    {
        _slot.button.onClick.AddListener(() => {
            if(UpgradeManager.instance.TryUpgradeCookSlot())
            Refresh(); 
        });
        _speed.button.onClick.AddListener(() => { UpgradeManager.instance.TryUpgradeSpeedUp(); Refresh(); });
        _orderBoard.button.onClick.AddListener(() => { UpgradeManager.instance.TryUpgradeCookBoard(); Refresh(); });
        _orderHint.button.onClick.AddListener(() => { UpgradeManager.instance.TryUpgradeOrderHint(); Refresh(); });
        _favorite.button.onClick.AddListener(() => { UpgradeManager.instance.TryUpgradeFavoriteSlot(); Refresh(); });
        Refresh();
    }
    private void Refresh()
    {
        if (UpgradeManager.instance == null || GoldManager.Instance == null) return;
        var um = UpgradeManager.instance;
        RefreshSection(_slot, um.CookSlotLevel, 2, um.SlotNextCost, um.CanUpgradeCookSlot());
        RefreshSection(_speed, um.SpeedUpLevel, 3, um.SpeedUpNextCost, um.CanUpgradeSpeedUp());
        RefreshSection(_orderBoard, um.OrderBoardLevel, 2, um.CookBoardNextCost, um.CanUpgradeCookBoard());
        RefreshSection(_orderHint, um.OrderHintLevel, 3, um.OrderHintNextCost, um.CanUpgradeOrderHint());
        RefreshSection(_favorite, um.FavoriteSlotCount, 2, um.FavoriteSlotNextCost, um.CanUpgradeFavorite());
    }
    private void RefreshSection(UpgradeSectionUI section, int current, 
        int max, int nextCost, bool canUpgrade)
    {
        section.levelText.text = $"Lv {current} / {max}";
        section.costText.text = nextCost > 0 ? $"Cost: {nextCost}" : "Max";
        section.button.interactable = canUpgrade;
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
