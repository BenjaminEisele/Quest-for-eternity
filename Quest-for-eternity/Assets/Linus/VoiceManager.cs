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
    public AudioClip[] openGame;
    public AudioClip[] startMatch;

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

    public bool PlaySoundClip(AudioClip audioClip, bool waitForLastLine)
    {
        if (latestSource != null)
        {
            if (!waitForLastLine)
            {
                EndLastetLine();
            }
            if (latestSource.gameObject.IsDestroyed() || !waitForLastLine)
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
        else if (latestSource == null)
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
        if (latestSource != null && !latestSource.gameObject.IsDestroyed())
        {
            Destroy(latestSource.gameObject);
        }
    }

    public void PlayTutorialLine(int lineIndex)
    {
        //if there is a voice line playing, destroy it
        if (latestSource != null)
        {
            if (!latestSource.gameObject.IsDestroyed())
            {
                Destroy(latestSource.gameObject);
            }
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
                if (latestSource != null)
                {
                    if (latestSource.clip != zombieKillPlayer && latestSource.clip != skeletonKillPlayer && latestSource.clip != skullKillPlayer && latestSource.clip != necroKillPlayer)
                    {
                        PlaySoundClip(zombieKillPlayer, false);
                    }
                }
                else
                {
                    PlaySoundClip(zombieKillPlayer, false);
                }

                break;

            case 1:
                if (latestSource != null)
                {
                    if (latestSource.clip != zombieKillPlayer && latestSource.clip != skeletonKillPlayer && latestSource.clip != skullKillPlayer && latestSource.clip != necroKillPlayer)
                    {
                        PlaySoundClip(skeletonKillPlayer, false);
                    }
                }
                else
                {
                    PlaySoundClip(skeletonKillPlayer, false);
                }
                break;

            case 2:
                if (latestSource != null)
                {
                    if (latestSource.clip != zombieKillPlayer && latestSource.clip != skeletonKillPlayer && latestSource.clip != skullKillPlayer && latestSource.clip != necroKillPlayer)
                    {
                        PlaySoundClip(skullKillPlayer, false);
                    }
                }
                else
                {
                    PlaySoundClip(skullKillPlayer, false);
                }
                break;

            case 3:
                if (latestSource != null)
                {
                    if (latestSource.clip != zombieKillPlayer && latestSource.clip != skeletonKillPlayer && latestSource.clip != skullKillPlayer && latestSource.clip != necroKillPlayer)
                    {
                        PlaySoundClip(necroKillPlayer, false);
                    }
                }
                else
                {
                    PlaySoundClip(necroKillPlayer, false);
                }
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
                    bool didPlay = PlaySoundClip(zombieSpawns, true);
                    zombieSpawned = didPlay;
                    
                }
                
                break;

            case 1:
                if (!skeletonSpawned)
                {
                    bool didPlay = PlaySoundClip(skeletonSpawns, true);
                    skeletonSpawned = didPlay;
                }
                break;

            case 2:
                if (!skullSpawned)
                {
                    bool didPlay = PlaySoundClip(skullSpawns, true);
                    skullSpawned = didPlay;
                }
                break;

            case 3:
                PlaySoundClip(necroSpawns, true);
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
                    bool didPlay = PlaySoundClip(zombieAttacks, true);
                    zombieAttacked = didPlay;
                }

                break;

            case 1:
                if (!skeletonAttacked)
                {
                    bool didPlay = PlaySoundClip(skeletonAttacks, true);
                    skeletonAttacked = didPlay;
                }
                break;

            case 2:
                if (!skullAttacked)
                {
                    bool didPlay = PlaySoundClip(skullAttacks, true);
                    skullAttacked = didPlay;
                }
                break;

            case 3:
                if (!necroAttacked)
                {
                    bool didPlay = PlaySoundClip(necroAttacks, true);
                    necroAttacked = didPlay; 
                }
                break;

        }
    }

    public void StrongAttackLine()
    {
        int rnd = Random.Range(0, strongAttacks.Length);
        PlaySoundClip(strongAttacks[rnd], true);
    }

    public void MissedAttackLine()
    {
        int rnd = Random.Range(0, missedAttacks.Length);
        PlaySoundClip(missedAttacks[rnd], true);
    }

    public void PlayersTurnLine()
    {
        int rnd = Random.Range(0, playerTurn.Length);
        PlaySoundClip(playerTurn[rnd], true);
    }

    public void OpenMenuLine()
    {
        int rnd = Random.Range(0, openMenu.Length);
        PlaySoundClip(openMenu[rnd], false);
    }

    public void OpenGameLine()
    {
        int rnd = Random.Range(0, openGame.Length);
        PlaySoundClip(openGame[rnd], false);
    }

    public void StartMatchLine()
    {
        int rnd = Random.Range(0, startMatch.Length);
        PlaySoundClip(startMatch[rnd], false);
    }
}
