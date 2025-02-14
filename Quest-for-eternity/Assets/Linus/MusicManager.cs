using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource soundObject;
    [SerializeField] private AudioClip MainMenuMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        instance.PlayMusic(MainMenuMusic, transform, 0.05f);
    }

    public void PlayMusic(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);

        //assign audio Clip
        audioSource.clip = audioClip;

        //loop music
        audioSource.loop = true;

        //assgin volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();
    }
}
