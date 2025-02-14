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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }

    public void PlaySoundClip(AudioClip audioClip, Transform spawnTransform, float volume, bool loop)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);

        //assign audio Clip
        audioSource.clip = audioClip;

        //assgin volume
        audioSource.volume = volume;

        //set if looped
        audioSource.loop = loop;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLenght = audioSource.clip.length;

        //remove gameobject when done playing
        Destroy(audioSource.gameObject, clipLenght);
    }

    public void ButtonSound()
    {
        instance.PlaySoundClip(buttonSound, transform, 1f, false);
    }

    public void SliderSound()
    {
        instance.PlaySoundClip(sliderSound, transform, 1f, false);
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
}
