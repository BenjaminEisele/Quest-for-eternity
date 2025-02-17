using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    public Toggle fullscreenToggle;
    [SerializeField] SoundFXManager soundFXManager;

    void Start()
    {
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        soundFXManager.ToggleSound();
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }
}
