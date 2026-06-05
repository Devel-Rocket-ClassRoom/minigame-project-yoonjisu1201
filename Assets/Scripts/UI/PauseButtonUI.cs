using UnityEngine;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _lobbyButton;
    [SerializeField] private LobbyConfirmPopupUI _confirmPopup;

    private bool _isPaused;

    private void Start()
    {
        _lobbyButton.gameObject.SetActive(false);
        _pauseButton.onClick.AddListener(OnPauseClicked);
        _lobbyButton.onClick.AddListener(OnLobbyClicked);
    }
    private void OnPauseClicked()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            SessionManager.instance.PauseTimer();
            _lobbyButton.gameObject.SetActive(true);
        }
        else
        {
            SessionManager.instance.ResumeTimer();
            _lobbyButton.gameObject.SetActive(false);
            _confirmPopup.Hide();
        }
    }
    private void OnLobbyClicked()
    {
        _confirmPopup.Show();
    }

}
