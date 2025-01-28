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
    public int myId;

    [SyncVar]
    public int[] enemyIdArray = new int[3];


    public readonly SyncList<int> enemyIdList = new SyncList<int>();
    public void RandomNumber(int howMany)
    {
        enemyIdList.Clear();
        //if (isServer)
        //{
      
        Debug.Log("Random Number beginning reached");
            for(int i = 0; i < howMany; i++)
            {
            
                enemyIdList.Add(Random.Range(0, databaseMultiplayerAccess.enemyList.Count));
                //enemyIdArray[i] = Random.Range(0, databaseMultiplayerAccess.enemyList.Count);
                Debug.Log($"generating id: {enemyIdArray[i]}");
            }
           //myId = Random.Range(0, databaseMultiplayerAccess.enemyList.Count);
      //  }           
    }
    public void TestVoid()
    {
        //namesList.Add("HELLO");
         myId = 65;
        enemyIdArray[0] = 65;
    }
    public void GenerateEnemies(int howManyEnemies)
    {
        Vector3 enemyPosition = spawnerPos.position;
        for (int i = 0; i < howManyEnemies; i++)
        {           
            GameObject enemyClone = Instantiate(enemyReference.gameObject, enemyPosition, Quaternion.identity);
            enemyClone.SetActive(true);
            enemyClone.GetComponent<EnemyScript>().EnemySetUp(enemyIdArray[i]);
            Debug.Log($"spawning enemy with id: {enemyIdArray[i]}");
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(4, 0, 0);                  
        }
        refereeScriptAccess.ResetChosenEnemy();
    }
}
