using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private AudioClip _titleBGM;
    [SerializeField] private AudioClip _buttonSFX;

    private void Start()
    {
        Debug.unityLogger.logEnabled = false;
        if (_continueButton != null)
            _continueButton.interactable = false;

        if (AuthManager.Instance != null)
            AuthManager.Instance.OnLoginStateChanged += OnLoginStateChanged;

        RefreshContinueButton();
        AudioManager.instance.PlayBGM(_titleBGM);
    }

    private void OnDestroy()
    {
        if (AuthManager.Instance != null)
            AuthManager.Instance.OnLoginStateChanged -= OnLoginStateChanged;
    }

    private void OnLoginStateChanged(bool isLoggedIn)
    {
        RefreshContinueButton();
    }

    public void RefreshContinueButton()
    {
        RefreshContinueButtonAsync().Forget();
    }

    private async UniTaskVoid RefreshContinueButtonAsync()
    {
        if (_continueButton == null)
            return;

        _continueButton.interactable = false;
        bool hasSave = SaveManager.Instance != null && await SaveManager.Instance.HasAnySaveAsync();
        _continueButton.interactable = hasSave;
    }

    public void OnContinueClicked()
    {
        OnContinueClickedAsync().Forget();
    }

    private async UniTaskVoid OnContinueClickedAsync()
    {
        if (_continueButton != null)
            _continueButton.interactable = false;

        bool loaded = SaveManager.Instance != null && await SaveManager.Instance.LoadLatestAsync();
        if (!loaded)
        {
            RefreshContinueButton();
            return;
        }

        AudioManager.instance.PlaySFX(_buttonSFX);
        AudioManager.instance.StopBGM();
        SceneManager.LoadScene("Lobby");
    }

    public void OnNewGameClicked()
    {
        OnNewGameClickedAsync().Forget();
    }

    private async UniTaskVoid OnNewGameClickedAsync()
    {
        // 매니저들은 DontDestroyOnLoad라 앱 실행 내내 살아있으므로
        if (SaveManager.Instance != null)
            await SaveManager.Instance.DeleteAllSaveAsync();

        ResetAllManagersForNewGame();

        CookingGuideManager.ResetGuideDone();
        DialogueManager.ResetSeenDialogues();
        AudioManager.instance.PlaySFX(_buttonSFX);
        AudioManager.instance.StopBGM();
        SceneManager.LoadScene("Lobby");
    }
    private void ResetAllManagersForNewGame()
    {
        GoldManager.Instance?.ResetAll();
        TruckRankManager.instance?.ResetAll();
        UpgradeManager.instance?.ResetAll();
        UnlockManager.instance?.ResetAll();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

}
