using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Button _continueButton;

    private void Start()
    {
        _continueButton.interactable = SaveManager.Instance.HasSave();
    }
    public void OnContinueClicked()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void OnNewGameClicked()
    {
        // 매니저들은 DontDestroyOnLoad라 앱 실행 내내 살아있으므로
        SaveManager.Instance.DeleteSave();
        SceneManager.LoadScene("Lobby");
    }
    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
