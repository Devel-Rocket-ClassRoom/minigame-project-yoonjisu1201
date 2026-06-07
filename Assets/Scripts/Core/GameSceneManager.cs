using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [SerializeField] private AudioClip _lobbyBGM;
    [SerializeField] private AudioClip _buttonSFX;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;

        AudioManager.instance.PlayBGM(_lobbyBGM);

        if (SaveManager.Instance.HasSave())
            SaveManager.Instance.Load();
        else
            ResetAllManagers();
    }

    public void PlayButtonSFX()
    {
        AudioManager.instance.PlaySFX(_buttonSFX);
    }

    private void ResetAllManagers()
    {
        GoldManager.Instance.ResetAll();
        TruckRankManager.instance.ResetAll();
        UpgradeManager.instance.ResetAll();
        UnlockManager.instance.ResetAll();
        DialogueManager.Instance?.Play("game_start");
    }
    public void GoToLobby()
    {
        GoldManager.Instance.CommitSession();
        TruckRankManager.instance.CommitSession();
        UnlockManager.instance.CommitSessionArtifacts();
        SaveManager.Instance.Save();
        SceneManager.LoadScene("Lobby");
    }

    public void GoToLobbyForfeit()
    {
        UnlockManager.instance.ResetSessionArtifacts();
        SceneManager.LoadScene("Lobby");
    }
    public void GoToCooking()
    {
        PlayButtonSFX();
        AudioManager.instance.StopBGM();
        SceneManager.LoadScene("Cooking");
    }
}
