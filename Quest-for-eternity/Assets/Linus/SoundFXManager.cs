using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundObject;

    public AudioClip dropdownSound;
    public AudioClip hoverSound;
    public AudioClip buttonSound;
    public AudioClip toggleSound;
    public AudioClip sliderSound;
    public AudioClip coinSound;
    public AudioSource latestSource;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }

    public void PlaySoundClip(AudioClip audioClip, Transform spawnTransform, float volume, bool setLatestSource)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);
        if(setLatestSource)
        {
            latestSource = audioSource;
        }

        //assign audio Clip
        audioSource.clip = audioClip;

        //assgin volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLenght = audioSource.clip.length;

        //remove gameobject when done playing
        Destroy(audioSource.gameObject, clipLenght);
    }

    public void SliderSound()
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);

        latestSource = audioSource;

        //assign audio Clip
        audioSource.clip = sliderSound;

        //assgin volume
        audioSource.volume = 1f;

        //loop sound
        audioSource.loop = true;

        //play sound
        audioSource.Play();
    }

    public void ButtonSound()
    {
        instance.PlaySoundClip(buttonSound, transform, 1f, false);
    }

    public void StopLatestSound ()
    {
        Destroy(latestSource.gameObject);
    }

    public void DropdownSound()
    {
        instance.PlaySoundClip(dropdownSound, transform, 1f, false);
    }

    public void ToggleSound()
    {
        instance.PlaySoundClip(toggleSound, transform, 1f, false);
    }

        public void HoverSound()
    {
        instance.PlaySoundClip(hoverSound, transform, 1f, false);
    }

    public void CoinSound()
    {
        instance.PlaySoundClip(coinSound, transform, 1f, false);
    }
}
