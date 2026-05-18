using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void GoToCooking()
    {
        SceneManager.LoadScene("Cooking");
    }
}
