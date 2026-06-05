using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;

        if (SaveManager.Instance.HasSave())
            SaveManager.Instance.Load();
        else
            ResetAllManagers();
    }

    private void ResetAllManagers()
    {
        GoldManager.Instance.ResetAll();
        TruckRankManager.instance.ResetAll();
        UpgradeManager.instance.ResetAll();
        UnlockManager.instance.ResetAll();
    }
    public void GoToLobby()
    {
        SaveManager.Instance.Save();
        SceneManager.LoadScene("Lobby");
    }
    public void GoToCooking()
    {
        SceneManager.LoadScene("Cooking");
    }
}
