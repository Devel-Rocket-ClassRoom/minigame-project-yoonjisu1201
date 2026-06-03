using UnityEngine;

public enum UpgradeType { CookSlot, SpeedUp, OrderBoard, OrderHint, Container }

//업그레이드별 필요 비용과 레벨 관리, 업드레이드 가능여부 체크, 골드 차감, 결과값 내보내기
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance { get; private set; }

    [SerializeField] private BalanceConfigSO _balanceConfig;

    private int[] COOKSLOT_COSTS => _balanceConfig.cookSlotCosts;
    private int[] SPEEDUP_COSTS => _balanceConfig.speedUpCosts;
    private int[] COOK_BOARD_COSTS => _balanceConfig.cookBoardCosts;
    private int[] ORDER_HINT_COSTS => _balanceConfig.orderHintCosts;
    private int[] CONTAINER_SLOT_COSTS => _balanceConfig.containerSlotCosts;

    private int _cookSlotLevel = 0;
    private int _speedUpLevel = 0;
    private int _cookBoardLevel = 0;
    private int _orderHintLevel = 0;
    private int _containerSlotLevel = 0;

    public int ActiveSlotCount => _cookSlotLevel + 1;
    public float CookingSpeedMultiplier =>
        _speedUpLevel > 0 ? _balanceConfig.speedUpMultipliers[_speedUpLevel - 1] : 1f;

    public int CookSlotLevel => _cookSlotLevel;
    public int SpeedUpLevel => _speedUpLevel;
    public int OrderBoardLevel => _cookBoardLevel;
    public int OrderHintLevel => _orderHintLevel;
    public int ContainerSlotCount => _containerSlotLevel;
    public int SlotNextCost => _cookSlotLevel < COOKSLOT_COSTS.Length ?
        COOKSLOT_COSTS[_cookSlotLevel] : 0;
    public int SpeedUpNextCost => _speedUpLevel < SPEEDUP_COSTS.Length ?
        SPEEDUP_COSTS[_speedUpLevel] : 0;
    public int CookBoardNextCost => _cookBoardLevel < COOK_BOARD_COSTS.Length ?
        COOK_BOARD_COSTS[_cookBoardLevel] : 0;
    public int OrderHintNextCost => _orderHintLevel < ORDER_HINT_COSTS.Length ? 
        ORDER_HINT_COSTS[_orderHintLevel] : 0;
    public int ContainerSlotNextCost => _containerSlotLevel < CONTAINER_SLOT_COSTS.Length ?
        CONTAINER_SLOT_COSTS[_containerSlotLevel] : 0;

    private void Awake()
    {
        if(instance != null)
        { 
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //업그레이드 메서드 패턴
    public bool CanUpgradeCookSlot() =>
        _cookSlotLevel < COOKSLOT_COSTS.Length &&
        GoldManager.Instance.TotalGold >= COOKSLOT_COSTS[_cookSlotLevel];

    public bool TryUpgradeCookSlot()
    {
        if (!CanUpgradeCookSlot()) return false;
        GoldManager.Instance.TrySpendGold(COOKSLOT_COSTS[_cookSlotLevel]);
        _cookSlotLevel++;
        return true;
    }
    public bool CanUpgradeSpeedUp() =>
        _speedUpLevel < SPEEDUP_COSTS.Length &&
        GoldManager.Instance.TotalGold >= SPEEDUP_COSTS[_speedUpLevel];

    public bool TryUpgradeSpeedUp()
    {
        if (!CanUpgradeSpeedUp()) return false;
        GoldManager.Instance.TrySpendGold(SPEEDUP_COSTS[_speedUpLevel]);
        _speedUpLevel++;
        return true;
    }
    public bool CanUpgradeCookBoard() =>
        _cookBoardLevel < COOK_BOARD_COSTS.Length &&
        GoldManager.Instance.TotalGold >= COOK_BOARD_COSTS[_cookBoardLevel];

    public bool TryUpgradeCookBoard()
    {
        if (!CanUpgradeCookBoard()) return false;
        GoldManager.Instance.TrySpendGold(COOK_BOARD_COSTS[_cookBoardLevel]);
        _cookBoardLevel++;
        return true;
    }
    public bool CanUpgradeOrderHint() =>
        _orderHintLevel < ORDER_HINT_COSTS.Length &&
        GoldManager.Instance.TotalGold >= ORDER_HINT_COSTS[_orderHintLevel];

    public bool TryUpgradeOrderHint()
    {
        if (!CanUpgradeOrderHint()) return false;
        GoldManager.Instance.TrySpendGold(ORDER_HINT_COSTS[_orderHintLevel]);
        _orderHintLevel++;
        return true;
    }
    public bool CanUpgradeContainer() =>
        _containerSlotLevel < CONTAINER_SLOT_COSTS.Length &&
        GoldManager.Instance.TotalGold >= CONTAINER_SLOT_COSTS[_containerSlotLevel];
    public bool TryUpgradeContainerSlot()
    {
        if (!CanUpgradeContainer()) return false;
        GoldManager.Instance.TrySpendGold(CONTAINER_SLOT_COSTS[_containerSlotLevel]);
        _containerSlotLevel++;
        return true;
    }

    // UpgradePanelUI에서 타입별로 통합 접근하기 위한 헬퍼
    public int GetCurrentLevel(UpgradeType type) => type switch
    {
        UpgradeType.CookSlot => _cookSlotLevel,
        UpgradeType.SpeedUp => _speedUpLevel,
        UpgradeType.OrderBoard => _cookBoardLevel,
        UpgradeType.OrderHint => _orderHintLevel,
        UpgradeType.Container => _containerSlotLevel,
        _ => 0
    };

    public int[] GetCosts(UpgradeType type) => type switch
    {
        UpgradeType.CookSlot => COOKSLOT_COSTS,
        UpgradeType.SpeedUp => SPEEDUP_COSTS,
        UpgradeType.OrderBoard => COOK_BOARD_COSTS,
        UpgradeType.OrderHint => ORDER_HINT_COSTS,
        UpgradeType.Container => CONTAINER_SLOT_COSTS,
        _ => new int[0]
    };

    public bool TryUpgrade(UpgradeType type) => type switch
    {
        UpgradeType.CookSlot => TryUpgradeCookSlot(),
        UpgradeType.SpeedUp => TryUpgradeSpeedUp(),
        UpgradeType.OrderBoard => TryUpgradeCookBoard(),
        UpgradeType.OrderHint => TryUpgradeOrderHint(),
        UpgradeType.Container => TryUpgradeContainerSlot(),
        _ => false
    };

    public bool CanUpgrade(UpgradeType type) => type switch
    {
        UpgradeType.CookSlot => CanUpgradeCookSlot(),
        UpgradeType.SpeedUp => CanUpgradeSpeedUp(),
        UpgradeType.OrderBoard => CanUpgradeCookBoard(),
        UpgradeType.OrderHint => CanUpgradeOrderHint(),
        UpgradeType.Container => CanUpgradeContainer(),
        _ => false
    };
}
