using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("HerkusScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}