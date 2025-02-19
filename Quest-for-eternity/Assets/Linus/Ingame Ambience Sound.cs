using UnityEngine;

public class IngameAmienceSound : MonoBehaviour
{
    void Start()
    {
        MusicManager.instance.PlayAmbience();
    }
}
