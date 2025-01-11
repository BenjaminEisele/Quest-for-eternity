using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPosition;

    void Start()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition);
    }
}
