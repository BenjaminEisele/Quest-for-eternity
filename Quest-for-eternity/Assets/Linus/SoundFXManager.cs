using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundObject;

    public AudioClip dropdownSound;
    public AudioClip hoverSound;
    public AudioClip buttonSound;
    public AudioClip toggleSound;
    public AudioClip sliderSound;
    public AudioClip coinSound;
    public AudioClip drawSound;
    public AudioClip playCardSound;
    public AudioClip shuffleSound;
    public AudioClip flipSound;
    public AudioClip enemySpawnSound;
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip lootAppearsSound;

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

        //assign audio Clip
        audioSource.clip = audioClip;

        //assgin volume
        audioSource.volume = 1f;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLenght = audioSource.clip.length;

        //remove gameobject when done playing
        Destroy(audioSource.gameObject, clipLenght);
    }

    public void SliderSound()
    {
        //Spawn Gameobject
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);

        latestSource = audioSource;

        //assign audio Clip
        audioSource.clip = sliderSound;

        //assgin volume
        audioSource.volume = 1f;

        //loop sound
        audioSource.loop = true;

        //play sound
        audioSource.Play();
    }

    public void StopLatestSound ()
    {
        Destroy(latestSource.gameObject);
    }

    public void ButtonSound()
    {
        PlaySoundClip(buttonSound);
    }


    public void DropdownSound()
    {
        PlaySoundClip(dropdownSound);
    }

    public void ToggleSound()
    {
        PlaySoundClip(toggleSound);
    }

    public void HoverSound()
    {
        PlaySoundClip(hoverSound);
    }

    public void EndTurnSound()
    {
        PlaySoundClip(coinSound);
    }

    public void DrawSound()
    {
        PlaySoundClip(drawSound);
    }

    public void PlayCardSound()
    {
        PlaySoundClip(playCardSound);
    }

    public void ShuffleSound()
    {
        PlaySoundClip(shuffleSound);
    }

    public void FlipSound()
    {
        PlaySoundClip(flipSound);
    }

    public void EnemySpawnSound()
    {
        PlaySoundClip(enemySpawnSound);
    }

    public void HitSound()
    {
        PlaySoundClip(hitSound);
    }

    public void MissSound()
    {
        PlaySoundClip(missSound);
    }

    public void LootAppearsSound()
    {
        PlaySoundClip(lootAppearsSound);
    }
}
