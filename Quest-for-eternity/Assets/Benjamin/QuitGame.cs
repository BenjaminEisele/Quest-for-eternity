using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public void QuitGameFunction()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        Debug.Log("exit ingame " + PlayerPrefs.GetInt("fullscreen"));

        if (PlayerPrefab != null)
        {
            PlayerPrefab.SetActive(false);
        }
        
    }
}
