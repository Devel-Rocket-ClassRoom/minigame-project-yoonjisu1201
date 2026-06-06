using UnityEngine;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _lobbyButton;
    [SerializeField] private CanvasGroup _lobbyCanvasGroup;
    [SerializeField] private LobbyConfirmPopupUI _confirmPopup;
    [SerializeField] private GuestSpawner[] _spawners;

    public static bool IsPaused { get; private set; }

    private void Start()
    {
        SetLobbyButtonVisible(false);
        _pauseButton.onClick.AddListener(OnPauseClicked);
        _lobbyButton.onClick.AddListener(OnLobbyClicked);
    }
    private void OnPauseClicked()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            SessionManager.instance.PauseTimer();
            foreach (var spawner in _spawners) spawner.PauseSpawning();
            Invoke(nameof(ShowLobbyButton), 0f);
        }
        else
        {
            SessionManager.instance.ResumeTimer();
            foreach (var spawner in _spawners) spawner.ResumeSpawning();
            SetLobbyButtonVisible(false);
            _confirmPopup.Hide();
        }
    }
    private void ShowLobbyButton() => SetLobbyButtonVisible(true);

    private void SetLobbyButtonVisible(bool visible)
    {
        _lobbyCanvasGroup.alpha = visible ? 1f : 0f;
        _lobbyCanvasGroup.interactable = visible;
        _lobbyCanvasGroup.blocksRaycasts = visible;
    }
    private void OnLobbyClicked()
    {
        _confirmPopup.Show();
    }
    private void OnDestroy()
    {
        IsPaused = false;
    }

}
