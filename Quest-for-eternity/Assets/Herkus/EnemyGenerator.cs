using UnityEngine;
using Mirror;

public class EnemyGenerator : NetworkBehaviour
{

    [SerializeField]
    EnemyScript enemyReference;

    public Transform spawnerPos;

    public RefereeScript refereeScriptAccess;

    //public DatabaseMultiplayer databaseMultiplayerAccess;

    [SyncVar]
    private int myId;

    public void RandomNumber()
    {
        if (isServer)
        {
            //myId = Random.Range(0, databaseMultiplayerAccess.enemyList.Count);
        }           
    }

    public void GenerateEnemies(int howManyEnemies)
    {
        Vector3 enemyPosition = spawnerPos.position;
        for (int i = 0; i < howManyEnemies; i++)
        {           
            GameObject enemyClone = Instantiate(enemyReference.gameObject, enemyPosition, Quaternion.identity);
            enemyClone.SetActive(true);
            enemyClone.GetComponent<EnemyScript>().EnemySetUp(myId);
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(4, 0, 0);                  
        }
        refereeScriptAccess.ResetChosenEnemy();
    }
}
