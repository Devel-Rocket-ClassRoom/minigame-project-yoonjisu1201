using UnityEngine;
using UnityEngine.UI;

public class GuidePanelUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private RankUpPopupUI _rankUpPopup;

    private const int TRIGGER_RANK = 4;

    private void Start()
    {
        _panel.SetActive(false);
        _openButton.gameObject.SetActive(TruckRankManager.instance.CurrentRank >= TRIGGER_RANK);

        _openButton.onClick.AddListener(() => _panel.SetActive(true));
        _closeButton.onClick.AddListener(() => _panel.SetActive(false));

        if (_rankUpPopup != null)
            _rankUpPopup.OnClosed += OnRankUpPopupClosed;
    }

    private void OnDestroy()
    {
        if (_rankUpPopup != null)
            _rankUpPopup.OnClosed -= OnRankUpPopupClosed;
    }

    private void OnRankUpPopupClosed()
    {
        if (TruckRankManager.instance.CurrentRank == TRIGGER_RANK)
        {
            _openButton.gameObject.SetActive(true);
            _panel.SetActive(true);
        }
    }
}
