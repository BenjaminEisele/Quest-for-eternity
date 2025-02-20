using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager instance;

    [SerializeField] private AudioSource soundObject;

    public AudioClip[] tutorialLines;

    public AudioClip zombieKillPlayer;
    public AudioClip skeletonKillPlayer;
    public AudioClip skullKillPlayer;
    public AudioClip necroKillPlayer;

    public AudioSource latestSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }

    public void PlaySoundClip(AudioClip audioClip, bool loop)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);

        latestSource = audioSource;

        //assign audio Clip
        audioSource.clip = audioClip;

        //assgin volume
        audioSource.volume = 1f;

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
        PlaySoundClip(tutorialLines[lineIndex], false);
    }

    public void KillPlayerLine(int enemyID)
    {
        Debug.Log("function executed : " + enemyID);
        switch (enemyID)
        {
            case 0:

                PlaySoundClip(zombieKillPlayer, false);
                break;

            case 1:

                PlaySoundClip(skeletonKillPlayer, true);
                break;

            case 2:

                PlaySoundClip(skullKillPlayer, true);
                break;

            case 3:

                PlaySoundClip(necroKillPlayer, true);
                break;
        }
    }

    public void StopTutorialLine()
    {
        if (!latestSource.IsDestroyed())
        {
            Destroy(latestSource.gameObject);
        }
    }
}
