using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public void QuitGameFunction()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        PlayerPrefab.SetActive(false);
    }
}
