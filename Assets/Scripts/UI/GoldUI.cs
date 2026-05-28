using UnityEngine;
using TMPro;

//스크립트 하나로 로비씬, 영업씬에서 골드UI 텍스트 관리 (플래그 사용)
public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private bool _showSesionGold;

    private void OnEnable()
    {
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged += Refresh;
            Refresh();
        }
        Refresh();
    }
    private void OnDisable()
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged -= Refresh;
    }

    public void Refresh()
    {
        if (GoldManager.Instance == null) return;
        int gold = _showSesionGold ? GoldManager.Instance.SessionGold
            : GoldManager.Instance.TotalGold;
        _goldText.text = $"{gold:#,###} G";
    }
}
