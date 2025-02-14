using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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
