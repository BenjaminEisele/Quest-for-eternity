using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro.Examples;
using NUnit.Framework.Internal;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Mirror.BouncyCastle.Tsp;
using System;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] VoiceManager voiceManager;
    [SerializeField] MusicManager musicManager;
    void Awake()
    {
        voiceManager.OpenGameLine();
        musicManager.PlayAmbience();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}