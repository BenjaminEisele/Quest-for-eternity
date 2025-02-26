using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] AudioSource soundObject;
    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip CreditsMusic;
    [SerializeField] AudioClip mainMenuSong;
    private AudioSource mainMusicSource;
    private AudioSource creditsSource;
    private AudioSource mainMenuSongSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public AudioSource PlayMusic(AudioClip audioClip, float volume)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);

        //assign audio Clip
        audioSource.clip = audioClip;

        //loop music
        audioSource.loop = true;

        //assgin volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        return audioSource;
    }

    public void StopMusic (AudioSource audioSource)
    {
        if(audioSource.gameObject != null)
        {
        if(!audioSource.gameObject.IsDestroyed())
        {
            Destroy(audioSource.gameObject);
        }
        }
    }

    public void PlayAmbience()
    {
        mainMusicSource = PlayMusic(mainMenuMusic, 0.15f);
    }

    public void StopMainMusic()
    {
        StopMusic(mainMusicSource);
    }

    public void PlayCredits()
    {
        //creditsSource = PlayMusic(CreditsMusic, 1f);
    }
        public void StopCreditsMusic()
    {
        //StopMusic(creditsSource);
    }

        public void PlayMainMenuSong()
    {
        //mainMenuSongSource = PlayMusic(mainMenuSong, 1f);
    }
        public void StopMainMenuSong()
    {
        //StopMusic(mainMenuSongSource);
    }
}
