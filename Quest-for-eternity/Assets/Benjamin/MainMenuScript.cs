using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro.Examples;
using NUnit.Framework.Internal;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Mirror.BouncyCastle.Tsp;

public class MainMenuScript : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer audioMixer;

    public Toggle fullscreenToggle;

    public TMPro.TMP_Dropdown qualityDropdown;
    public int qualityIndexSave;

    public TMPro.TMP_Dropdown resolutionDropdown;



    
    void Start()
    {
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
        volumeSlider.value = PlayerPrefs.GetFloat("volume");

        if (PlayerPrefs.GetInt("quality") != 0 && PlayerPrefs.GetInt("quality") != 1 && PlayerPrefs.GetInt("quality") != 2 && PlayerPrefs.GetInt("quality") != 3)
        {
            PlayerPrefs.SetInt("quality", 1);
        }
        qualityDropdown.value = PlayerPrefs.GetInt("quality");

        resolutionDropdown.value = PlayerPrefs.GetInt("resolution");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityIndexSave = qualityIndex;
        PlayerPrefs.SetInt("quality", qualityIndexSave);
    }
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution (int resolutionIndex)
    {
        switch(resolutionIndex)
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