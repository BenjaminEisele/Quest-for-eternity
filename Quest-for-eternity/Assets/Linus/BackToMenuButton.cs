using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");

        if(PlayerPrefab != null )
        {
            PlayerPrefab.SetActive(false);
        }
    }
}
