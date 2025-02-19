using UnityEngine;

public class IngameAmbienceSound : MonoBehaviour
{
    void Awake()
    {
        MusicManager.instance.PlayMainMenuMusic();
    }
}
