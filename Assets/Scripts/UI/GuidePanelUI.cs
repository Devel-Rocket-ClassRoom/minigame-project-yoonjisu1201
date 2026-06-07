using System.Collections;
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

        _openButton.onClick.AddListener(() =>
        {
            GameSceneManager.Instance?.PlayButtonSFX();
            _panel.SetActive(true);
        });
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
            StartCoroutine(CoShowAfterDialogue());
        }
    }

    private IEnumerator CoShowAfterDialogue()
    {
        yield return null; // 대화 시작 대기
        while (DialogueManager.Instance != null && DialogueManager.Instance.IsDialoguePlaying)
            yield return null;
        _panel.SetActive(true);
    }
}
