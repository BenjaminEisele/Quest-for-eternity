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

    public AudioClip zombieSpawns;
    public AudioClip skeletonSpawns;
    public AudioClip skullSpawns;
    public AudioClip necroSpawns;

    public AudioClip zombieAttacks;
    public AudioClip skeletonAttacks;
    public AudioClip skullAttacks;
    public AudioClip necroAttacks;

    public bool zombieAttacked = false;
    public bool skeletonAttacked = false;
    public bool skullAttacked = false;
    public bool necroAttacked = false;

    public bool zombieSpawned = false;
    public bool skeletonSpawned = false;
    public bool skullSpawned = false;

    public AudioSource latestSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }

    public void PlaySoundClip(AudioClip audioClip)
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);

        latestSource = audioSource;

        //assign audio Clip
        audioSource.clip = audioClip;

        //assgin volume
        audioSource.volume = 1f;

        //set if looped
        audioSource.loop = false;

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
        PlaySoundClip(tutorialLines[lineIndex]);
    }

    public void KillPlayerLine(int enemyID)
    {
        switch (enemyID)
        {
            case 0:

                PlaySoundClip(zombieKillPlayer);
                break;

            case 1:

                PlaySoundClip(skeletonKillPlayer);
                break;

            case 2:

                PlaySoundClip(skullKillPlayer);
                break;

            case 3:

                PlaySoundClip(necroKillPlayer);
                break;
        }
    }

    public void EnemySpawnLine(int enemyID)
    {
        switch (enemyID)
        {
            case 0:
                if(!zombieSpawned)
                {
                    PlaySoundClip(zombieSpawns);
                    zombieSpawned = true;
                }
                
                break;

            case 1:
                if (!skeletonSpawned)
                {
                    PlaySoundClip(skeletonSpawns);
                    skeletonSpawned = true;
                }
                break;

            case 2:
                if (!skullSpawned)
                {
                    PlaySoundClip(skullSpawns);
                    skullSpawned = true;
                }
                break;

            case 3:
                PlaySoundClip(necroSpawns);
                break;
        }
    }

    public void EnemyAttackLine(int enemyID)
    {
        switch (enemyID)
        {
            case 0:
                if (!zombieAttacked)
                {
                    PlaySoundClip(zombieAttacks);
                    zombieAttacked = true;
                }

                break;

            case 1:
                if (!skeletonAttacked)
                {
                    PlaySoundClip(skeletonAttacks);
                    skeletonAttacked = true;
                }
                break;

            case 2:
                if (!skullAttacked)
                {
                    PlaySoundClip(skullAttacks);
                    skullAttacked = true;
                }
                break;

            case 3:
                if (!necroAttacked)
                {
                    PlaySoundClip(necroAttacks);
                    necroAttacked = true; 
                }
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
