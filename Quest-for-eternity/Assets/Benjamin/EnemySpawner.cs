using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    public Transform spawnPosition;
    public RefereeScript refereeScriptAccess;
    public DatabaseMultiplayer databaseMultiplayerAccess;
    [SyncVar]
    public int enemyId;

    void Start()
    {
        SpawnEnemy(2);        
    }

    public void RandomNumber()
    {
        if (isServer)
        {
            enemyId = Random.Range(0, databaseMultiplayerAccess.enemyPrefabList.Count);
        }
    }

    private void SpawnEnemy(int howManyEnemies)
    {
        Vector3 enemyPosition = spawnPosition.position;
        for (int i = 0; i < howManyEnemies; i++)
        {
            RandomNumber();
            GameObject enemyClone = Instantiate(databaseMultiplayerAccess.enemyPrefabList[enemyId], enemyPosition, Quaternion.identity);
            enemyClone.SetActive(true);
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(3, 0, 0);
        }
    }
}