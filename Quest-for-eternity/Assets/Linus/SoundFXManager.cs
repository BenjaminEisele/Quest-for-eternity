using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundObject;
    [SerializeField] private AudioClip buttonSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }

    public void PlaySoundClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);

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

    public void ClickSound()
    {
        instance.PlaySoundClip(buttonSound, transform, 1f);
    }
}
