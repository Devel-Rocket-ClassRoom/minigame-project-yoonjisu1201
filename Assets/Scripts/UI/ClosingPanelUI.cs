using UnityEngine;
using TMPro;

public class ClosingPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionGoldText;

    private void OnEnable()
    {
        int earned = GoldManager.Instance.SessionGold;
        _sessionGoldText.text = $"{earned}G";
    }
}
