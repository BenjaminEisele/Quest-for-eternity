using UnityEngine;

public class IngameAmbienceSound : MonoBehaviour
{
    public void Awake()
    {
        Debug.Log("Awake");
        MusicManager.instance.PlayAmbience();
    }
} 

