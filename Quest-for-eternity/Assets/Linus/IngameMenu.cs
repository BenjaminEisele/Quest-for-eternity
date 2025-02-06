using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro.Examples;
using NUnit.Framework.Internal;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class IngameMenu : MonoBehaviour
{
    public bool pauseMenuOpen = false;

    public Slider volumeSlider;
    public float volumeSave;
    public AudioMixer audioMixer;

    public Toggle fullscreenToggle;
    public int fullscreenSave = 1;

    public TMPro.TMP_Dropdown qualityDropdown;
    public int qualityIndexSave;

    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDropdown;

    void Start()
    {
        fullscreenSave = PlayerPrefs.GetInt("fullscreen");
        fullscreenToggle.isOn = fullscreenSave == 1;

        volumeSlider.value = PlayerPrefs.GetFloat("volume");

        if (PlayerPrefs.GetInt("quality") != 0 && PlayerPrefs.GetInt("quality") != 1 && PlayerPrefs.GetInt("quality") != 2 && PlayerPrefs.GetInt("quality") != 3)
        {
            PlayerPrefs.SetInt("quality", 1);
        }
        qualityDropdown.value = PlayerPrefs.GetInt("quality");

        resolutionDropdown.value = PlayerPrefs.GetInt("resolution");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuOpen = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        pauseMenuOpen = false;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        volumeSave = volume;
        PlayerPrefs.SetFloat("volume", volumeSave);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityIndexSave = qualityIndex;
        PlayerPrefs.SetInt("quality", qualityIndexSave);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        fullscreenSave = isFullscreen ? 1 : 0;
        PlayerPrefs.SetInt("fullscreen", fullscreenSave);
    }

    public void SetResolution(int resolutionIndex)
    {
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
