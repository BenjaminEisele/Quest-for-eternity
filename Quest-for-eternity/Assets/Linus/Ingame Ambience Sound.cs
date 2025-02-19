using UnityEngine;

public class IngameAmienceSound : MonoBehaviour
{
    void Awake()
    {
        MusicManager.instance.PlayAmbience();
    }
}
