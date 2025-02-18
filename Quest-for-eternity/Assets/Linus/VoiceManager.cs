using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager instance;

    [SerializeField] private AudioSource soundObject;

    public AudioClip[] tutorialLines;

    public AudioSource latestSource;

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

        latestSource = audioSource;

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

    public void PlayTutorialLine(int lineIndex)
    {
        if(!latestSource.IsDestroyed())
        {
            Destroy(latestSource.gameObject);
        }
        PlaySoundClip(tutorialLines[lineIndex], transform, 1f, false);
    }

    public void StopTutorialLine()
    {
        if (!latestSource.IsDestroyed())
        {
            Destroy(latestSource.gameObject);
        }
    }
}
