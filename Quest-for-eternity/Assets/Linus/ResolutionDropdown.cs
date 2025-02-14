using UnityEngine;
using UnityEngine.EventSystems;

public class ResolutionDropdown : MonoBehaviour, IPointerClickHandler
{
    private bool gameOpening = true;
    public TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] SoundFXManager soundFXManager;

    void Start()
    {
        resolutionDropdown.value = PlayerPrefs.GetInt("resolution");
        gameOpening = false;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        soundFXManager.DropdownSound();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (!gameOpening)
        {
            soundFXManager.DropdownSound();
        }
        switch (resolutionIndex)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1366, 768, Screen.fullScreen);
                break;

            case 2:
                Screen.SetResolution(1280, 1024, Screen.fullScreen);
                break;

            case 3:
                Screen.SetResolution(1024, 768, Screen.fullScreen);
                break;
        }
        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }
}
