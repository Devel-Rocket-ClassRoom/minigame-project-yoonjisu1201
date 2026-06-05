using UnityEngine;
using UnityEngine.UI;

public class LobbyConfirmPopupUI : MonoBehaviour
{
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private GameSceneManager _gameSceneManager;

    private void Start()
    {
        gameObject.SetActive(false);
        _confirmButton.onClick.AddListener(() => _gameSceneManager.GoToLobby());
        _cancelButton.onClick.AddListener(Hide);
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
