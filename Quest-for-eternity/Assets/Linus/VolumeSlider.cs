using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private bool gameOpening = true;
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
        if (!gameOpening)
        {
            soundFXManager.SliderSound();
        }
        audioMixer.SetFloat(volumeVar, Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat(volumeVar, volume);
    }
}
