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
using UnityEngine.EventSystems;

public class VolumeSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private bool gameOpening = true;
    private bool firstChange = true;
    public Slider slider;
    public AudioMixer audioMixer;
    public string volumeVar;
    [SerializeField] SoundFXManager soundFXManager;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeVar);
        audioMixer.SetFloat(volumeVar, Mathf.Log10(slider.value) * 20f);
        gameOpening = false;
    }

    public void SetVolume (float volume)
    {
        if (!gameOpening && firstChange)
        {
            soundFXManager.SliderSound();
            firstChange = false;
        }
        audioMixer.SetFloat(volumeVar, Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat(volumeVar, volume);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        firstChange = true;
    }
}
