using Cysharp.Threading.Tasks;
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

    private async UniTaskVoid Start()
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;

        AudioManager.instance.PlayBGM(_lobbyBGM);

        bool loaded = SaveManager.Instance != null && await SaveManager.Instance.LoadLatestAsync();
        if (!loaded)
            ResetAllManagers();

        ApplyCurrentRankUnlocks();
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

    private void ApplyCurrentRankUnlocks()
    {
        RankUnlockHandler unlockHandler = FindFirstObjectByType<RankUnlockHandler>();
        unlockHandler?.ApplyUnlocksUpToCurrentRank();
    }
    public void GoToLobby()
    {
        GoToLobbyAsync().Forget();
    }

    private async UniTaskVoid GoToLobbyAsync()
    {
        GoldManager.Instance.CommitSession();
        TruckRankManager.instance.CommitSession();
        UnlockManager.instance.CommitSessionArtifacts();

        if (SaveManager.Instance != null)
            await SaveManager.Instance.SaveWithBackupAsync();

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
