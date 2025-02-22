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

    public AudioClip[] strongAttacks;
    public AudioClip[] missedAttacks;
    public AudioClip[] playerTurn;
    public AudioClip[] openMenu;

    public int miscLineChance = 33;

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

    public bool PlaySoundClip(AudioClip audioClip)
    {
        //if there is a voice line playing, destroy it
        if (latestSource.IsDestroyed())
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

            return true;
        }
        else
        {
            return false;
        }
    }

    public void EndLastetLine()
    {
        Destroy(latestSource.gameObject);
    }

    public void PlayTutorialLine(int lineIndex)
    {
        //if there is a voice line playing, destroy it
        if (!latestSource.IsDestroyed())
        {
            Destroy(latestSource.gameObject);
        }

        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);

        latestSource = audioSource;

        //assign audio Clip
        audioSource.clip = tutorialLines[lineIndex];

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
                    bool didPlay = PlaySoundClip(zombieSpawns);
                    zombieSpawned = didPlay;
                }
                
                break;

            case 1:
                if (!skeletonSpawned)
                {
                    bool didPlay = PlaySoundClip(skeletonSpawns);
                    skeletonSpawned = didPlay;
                }
                break;

            case 2:
                if (!skullSpawned)
                {
                    bool didPlay = PlaySoundClip(skullSpawns);
                    skullSpawned = didPlay;
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
                    bool didPlay = PlaySoundClip(zombieAttacks);
                    zombieAttacked = didPlay;
                }

                break;

            case 1:
                if (!skeletonAttacked)
                {
                    bool didPlay = PlaySoundClip(skeletonAttacks);
                    skeletonAttacked = didPlay;
                }
                break;

            case 2:
                if (!skullAttacked)
                {
                    bool didPlay = PlaySoundClip(skullAttacks);
                    skullAttacked = didPlay;
                }
                break;

            case 3:
                if (!necroAttacked)
                {
                    bool didPlay = PlaySoundClip(necroAttacks);
                    necroAttacked = didPlay; 
                }
                break;

        }
    }

    public void StrongAttackLine()
    {
        int rnd = Random.Range(0, strongAttacks.Length);
        PlaySoundClip(strongAttacks[rnd]);
    }

    public void MissedAttackLine()
    {
        int rnd = Random.Range(0, missedAttacks.Length);
        PlaySoundClip(missedAttacks[rnd]);
    }

    public void PlayersTurnLine()
    {
        int rnd = Random.Range(0, playerTurn.Length);
        PlaySoundClip(playerTurn[rnd]);
    }

    public void OpenMenuLine()
    {
        int rnd = Random.Range(0, openMenu.Length);
        PlaySoundClip(openMenu[rnd]);
    }
}
