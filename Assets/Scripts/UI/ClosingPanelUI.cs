using UnityEngine;
using TMPro;

public class ClosingPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionGoldText;
    [SerializeField] private TextMeshProUGUI _sessionExpText;

    private void OnEnable()
    {
        int earned = GoldManager.Instance.SessionGold;
        _sessionGoldText.text = $"{earned}G";

        float expEarned = TruckRankManager.instance.SessionExp;
        _sessionExpText.text = $"+{expEarned}";
    }
}
