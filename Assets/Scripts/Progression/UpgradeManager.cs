using UnityEngine;

//업그레이드별 필요 비용과 레벨 관리, 업드레이드 가능여부 체크, 골드 차감, 결과값 내보내기
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance { get; private set; }
    //비용테이블 상수 - 실제 비용은 3주차에 조정
    private static readonly int[] COOKSLOT_COSTS = { 101, 102 };
    private static readonly int[] SPEEDUP_COSTS = { 201, 202, 203 };
    private static readonly int[] COOK_BOARD_COSTS = { 301, 302 };
    private static readonly int[] ORDER_HINT_COSTS = { 401, 402, 403 };
    private static readonly int[] FAVORITE_SLOT_COSTS = { 501, 502 };

    private int _cookSlotLevel = 0;
    private int _speedUpLevel = 0;
    private int _cookBoardLevel = 0;
    private int _orderHintLevel = 0;
    private int _favoriteSlotLevel = 0;

    public int ActiveSlotCount => _cookSlotLevel + 1;
    public float CookingSpeedMultiplier => _speedUpLevel switch
    {
        1 => 0.95f,
        2 => 0.92f,
        3 => 0.88f,  
        _ => 1f
    };

    public int CookSlotLevel => _cookSlotLevel;
    public int SpeedUpLevel => _speedUpLevel;
    public int OrderBoardLevel => _cookBoardLevel;
    public int OrderHintLevel => _orderHintLevel;
    public int FavoriteSlotCount => _favoriteSlotLevel;
    public int SlotNextCost => _cookSlotLevel < COOKSLOT_COSTS.Length ?
        COOKSLOT_COSTS[_cookSlotLevel] : 0;
    public int SpeedUpNextCost => _speedUpLevel < SPEEDUP_COSTS.Length ?
        SPEEDUP_COSTS[_speedUpLevel] : 0;
    public int CookBoardNextCost => _cookBoardLevel < COOK_BOARD_COSTS.Length ?
        COOK_BOARD_COSTS[_cookBoardLevel] : 0;
    public int OrderHintNextCost => _orderHintLevel < ORDER_HINT_COSTS.Length ? 
        ORDER_HINT_COSTS[_orderHintLevel] : 0;
    public int FavoriteSlotNextCost => _favoriteSlotLevel < FAVORITE_SLOT_COSTS.Length ?
        FAVORITE_SLOT_COSTS[_favoriteSlotLevel] : 0;

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
    public bool CanUpgradeFavorite() =>
        _favoriteSlotLevel < FAVORITE_SLOT_COSTS.Length &&
        GoldManager.Instance.TotalGold >= FAVORITE_SLOT_COSTS[_favoriteSlotLevel];
    public bool TryUpgradeFavoriteSlot()
    {
        if (!CanUpgradeFavorite()) return false;
        GoldManager.Instance.TrySpendGold(FAVORITE_SLOT_COSTS[_favoriteSlotLevel]);
        _favoriteSlotLevel++;
        return true;
    }
}
