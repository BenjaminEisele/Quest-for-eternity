using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource soundObject;
    [SerializeField] private AudioClip mainMenuMusic;
    private AudioSource mainMusicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        PlayMainMenuMusic();
    }

    public AudioSource PlayMusic(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);

        mainMusicSource = audioSource;

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

    public void PauseMusic (AudioSource audioSource)
    {
        Destroy(audioSource.gameObject);
    }

    public void PlayMainMenuMusic()
    {
        mainMusicSource = PlayMusic(mainMenuMusic, transform, 0.05f);
    }

    public void PauseMainMusic()
    {
        PauseMusic(mainMusicSource);
    }
}
