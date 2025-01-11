using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPosition;
    private int howManyEnemies;
    private int myId;
    public RefereeScript refereeScriptAccess;

    void Start()
    {
        //GameObject enemy = Instantiate(enemyPrefab, spawnPosition);
        //spawnPosition.position += new Vector3(3, 0, 0);


        Vector3 enemyPosition = spawnPosition.position;
        for (int i = 0; i < howManyEnemies; i++)
        {
            GameObject enemyClone = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            enemyClone.SetActive(true);
            enemyClone.GetComponent<EnemyScript>().EnemySetUp(myId);
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(3, 0, 0);
        }
    }
}