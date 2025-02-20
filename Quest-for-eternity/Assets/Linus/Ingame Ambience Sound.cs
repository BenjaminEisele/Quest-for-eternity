using UnityEngine;

public class IngameAmienceSound : MonoBehaviour
{
    [SerializeField] MusicManager musicManager;
    void Start()
    {
        musicManager.PlayAmbience();
    }
}
