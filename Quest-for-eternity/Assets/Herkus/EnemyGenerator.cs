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
    }

    public void TestVoid()
    {
        //namesList.Add("HELLO");
         myId = 65;
    }
    public void GenerateEnemies(int howManyEnemies)
    {
        Vector3 enemyPosition = spawnerPos.position;
        for (int i = 0; i < howManyEnemies; i++)
        {   
            //This is the problem
            GameObject enemyClone = Instantiate(enemyReference.gameObject, enemyPosition, Quaternion.identity);
            enemyClone.SetActive(true);
            if (refereeScriptAccess.waveCount == 0)
            {
                enemyClone.GetComponent<EnemyScript>().EnemySetUp(0);
            }
            else
            {
                enemyClone.GetComponent<EnemyScript>().EnemySetUp(enemyIdList[i]);
            }
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(4, 0, 0);                  
        }
        refereeScriptAccess.ResetChosenEnemy();
        refereeScriptAccess.RandomNumbersSetUpRoot();

    }
}
