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
        //Debug.unityLogger.logEnabled = false;
        _continueButton.interactable = SaveManager.Instance.HasSave();
        AudioManager.instance.PlayBGM(_titleBGM);
    }
    public void OnContinueClicked()
    {
        AudioManager.instance.PlaySFX(_buttonSFX);
        AudioManager.instance.StopBGM();
        SceneManager.LoadScene("Lobby");
    }

    public void OnNewGameClicked()
    {
        // 매니저들은 DontDestroyOnLoad라 앱 실행 내내 살아있으므로
        SaveManager.Instance.DeleteSave();
        PlayerPrefs.DeleteKey("guide_cooking_done");
        DialogueManager.ResetSeenDialogues();
        AudioManager.instance.PlaySFX(_buttonSFX);
        AudioManager.instance.StopBGM();
        SceneManager.LoadScene("Lobby");
    }
    public void OnQuitClicked()
    {
        Application.Quit();
    }

}
