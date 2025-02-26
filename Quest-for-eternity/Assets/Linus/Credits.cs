using UnityEngine;
using System.Collections;
using System;


public class Credits : MonoBehaviour
{
    private IEnumerator coroutine;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject settings;
    [SerializeField] MusicManager musicManager;

    void Start()
    {
       coroutine = WaitAndExit();
    }

    public void StartCreditsCounter()
    {
        musicManager.StopMainMusic();
        musicManager.PlayCredits();
        coroutine = WaitAndExit();
        StartCoroutine(coroutine); 
    }

    private IEnumerator WaitAndExit()
    {
        while (true)
        {
            yield return new WaitForSeconds(50.0f);
            credits.SetActive(false);
            menu.SetActive(true);
            settings.SetActive(true);
            musicManager.StopCreditMusic();
            musicManager.PlayAmbience();
            
        }
    }
}