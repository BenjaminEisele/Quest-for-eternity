using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro.Examples;
using NUnit.Framework.Internal;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Mirror.BouncyCastle.Tsp;
using Unity.VisualScripting;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public AudioMixer audioMixer;
    public string volumeVar;
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeVar);
    }
    
    public void SetVolume (float volume)
    {
        audioMixer.SetFloat(volumeVar, volume);
        PlayerPrefs.SetFloat(volumeVar, volume);
    }
}
