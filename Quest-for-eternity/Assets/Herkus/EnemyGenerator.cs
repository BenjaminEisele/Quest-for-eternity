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

    int firstId;


    public readonly SyncList<int> enemyIdList = new SyncList<int>();
    public void RandomNumber(int howMany)
    {
        enemyIdList.Clear();
        //if (isServer)
        //{
        int input;
        Debug.Log("Random Number beginning reached");
            for(int i = 0; i < howMany; i++)
            {
                input = Random.Range(0, 3);
                if(i == 0)
                {
                   firstId = input;
                }    
                enemyIdList.Add(input);
            }
           //myId = Random.Range(0, databaseMultiplayerAccess.enemyList.Count);
      //  }           
    }
    public void TestVoid()
    {
        //namesList.Add("HELLO");
         myId = 65;
    }
    public void GenerateEnemies(int howManyEnemies, bool forceSkeletonSpawning)
    {
        Vector3 enemyPosition = spawnerPos.position;
        for (int i = 0; i < howManyEnemies; i++)
        {           
            GameObject enemyClone = Instantiate(enemyReference.gameObject, enemyPosition, Quaternion.identity);
            enemyClone.SetActive(true);
            //if(forceSkeletonSpawning)
            //{

                //enemyClone.GetComponent<EnemyScript>().EnemySetUp(3);
            //}
           // else
            //{
                enemyClone.GetComponent<EnemyScript>().EnemySetUp(enemyIdList[i]);
                //enemyClone.GetComponent<EnemyScript>().EnemySetUp(3);
            //}
            Debug.Log($"spawning enemy with id: {enemyIdList[i]}");
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(4, 0, 0);                  
        }
        refereeScriptAccess.ResetChosenEnemy();
        RefereeScript.instance.RandomNumbersSetUpRoot();

    }
}
