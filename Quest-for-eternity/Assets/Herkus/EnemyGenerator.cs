using UnityEngine;
using Mirror;
using System.Collections.Generic;
public class EnemyGenerator : NetworkBehaviour
{

    [SerializeField]
    EnemyScript enemyReference;

    public Transform spawnerPos;

    public RefereeScript refereeScriptAccess;

    public DatabaseMultiplayer databaseMultiplayerAccess;

    [SyncVar]
    private int myId;

    [SyncVar]
    public List<int> enemyIdList;
    public void RandomNumber(int howMany)
    {
        enemyIdList.Clear();
        if (isServer)
        {
            for(int i = 0; i < howMany; i++)
            {
                enemyIdList.Add(Random.Range(0, databaseMultiplayerAccess.enemyList.Count));
            }
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
            enemyClone.GetComponent<EnemyScript>().EnemySetUp(enemyIdList[i]);
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(4, 0, 0);                  
        }
        refereeScriptAccess.ResetChosenEnemy();
    }
}
