using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public void QuitGameFunction()
    {
        SceneManager.LoadSceneAsync("MainMenu");

        if(PlayerPrefab != null)
        {
            PlayerPrefab.SetActive(false);
        }
        
    }
}
