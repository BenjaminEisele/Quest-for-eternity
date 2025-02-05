using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro.Examples;
using NUnit.Framework.Internal;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class MainMenuScript : MonoBehaviour
{
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

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("HerkusScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("volume", volume);
        volumeSave = volume;
        PlayerPrefs.SetFloat("volume", volumeSave);
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
        fullscreenSave = isFullscreen? 1 : 0;
        PlayerPrefs.SetInt("fullscreen", fullscreenSave);
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}