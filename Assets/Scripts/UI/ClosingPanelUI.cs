using UnityEngine;
using TMPro;

public class ClosingPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionGoldText;
    [SerializeField] private TextMeshProUGUI _sessionExpText;
    [SerializeField] private TextMeshProUGUI _sessionGuestCountText;

    private void OnEnable()
    {
        int earned = GoldManager.Instance.SessionGold;
        _sessionGoldText.text = $"{earned}G";

        float expEarned = TruckRankManager.instance.SessionExp;
        _sessionExpText.text = $"+{expEarned}";

        int guestCount = SessionManager.instance.SessionGuestCount;
        _sessionGuestCountText.text = $"{guestCount}";
    }
}
