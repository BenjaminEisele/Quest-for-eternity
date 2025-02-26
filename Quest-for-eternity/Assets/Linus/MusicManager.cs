using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Random=UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] VoiceManager voiceManager;

    [SerializeField] AudioSource soundObject;
    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip CreditsMusic;
    [SerializeField] AudioClip TutorialMusic;
    [SerializeField] AudioClip[] gameMusic;

    private AudioSource ambienceSource;
    private AudioSource creditsSource;
    private AudioSource tutorialSource;
    private AudioSource musicSource;

    bool gameMusicShouldplay = true;

    private void Awake()
    {
        PlayGameMusic();
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if(RefereeScript.instance.isGameOver)
        {
            StopGameMusic();
            voiceManager.RestartGameVoice();
        }
        if (musicSource == null)
        {
            if(gameMusicShouldplay)
            {
                PlayGameMusic();
            }
        }
    }

    public AudioSource PlayMusic(AudioClip audioClip, float volume, bool loop)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);

        //assign audio Clip
        audioSource.clip = audioClip;

        //loop music
        audioSource.loop = loop;

        //assgin volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        return audioSource;
    }

    public void StopMusic (AudioSource audioSource)
    {
        if(!audioSource.gameObject.IsDestroyed())
        {
            Destroy(audioSource.gameObject);
        }
    }

    public void PlayAmbience()
    {
        ambienceSource = PlayMusic(mainMenuMusic, 0.4f, true);
    }

    public void StopMainMusic()
    {
        StopMusic(ambienceSource);
    }

    public void PlayCredits()
    {
        creditsSource = PlayMusic(CreditsMusic, 1f, true);
        
    }

    public void StopCreditMusic()
    {
        StopMusic(creditsSource);
    }

    public void PlayTutorial()
    {
        tutorialSource = PlayMusic(TutorialMusic, 1f, true); 
        
    }

        public void StopTutorialMusic()
    {
        StopMusic(tutorialSource);
    }

    public void PlayGameMusic()
    {
        gameMusicShouldplay = true;
        int rnd = Random.Range(0, gameMusic.Length);
        musicSource = PlayMusic(gameMusic[rnd], 1f, false);
        float clipLenght = musicSource.clip.length;
        Destroy(musicSource.gameObject, clipLenght);
    }

    public void StopGameMusic()
    {
        gameMusicShouldplay = false;
        StopMusic(musicSource);
    }
}
