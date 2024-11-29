using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public void QuitGameFunction()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
