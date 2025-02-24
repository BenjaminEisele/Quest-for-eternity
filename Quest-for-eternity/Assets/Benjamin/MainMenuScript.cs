using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] VoiceManager voiceManager;
    [SerializeField] MusicManager musicManager;
    void Awake()
    {
        if (PlayerPrefs.GetInt("firstOpened") == 0)
        {
            voiceManager.OpenGameLine();
            PlayerPrefs.SetInt("firstOpened", 1);
        }
        musicManager.PlayAmbience();
    }
    public void QuitGame()
    {
        PlayerPrefs.SetInt("firstOpened", 0);
        Application.Quit();
    }
}