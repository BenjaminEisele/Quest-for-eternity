using UnityEngine;
using Mirror;
using System.Collections.Generic;
public class EnemyGenerator : NetworkBehaviour
{

    [SerializeField]
    EnemyScript enemyReference;

    [SerializeField]
    Transform spawnerPos;

    [SerializeField]
    RefereeScript refereeScriptAccess;

    int firstId;


    public readonly SyncList<int> enemyIdList = new SyncList<int>();
    public void RandomNumber(int howMany)
    {
        enemyIdList.Clear();
        int input;
            for(int i = 0; i < howMany; i++)
            {
                //DatabaseMultiplayer databaseMultiplayer;
                //databaseMultiplayer.enemyList[0].lootCardId
                input = Random.Range(0, 3);
                if(i == 0)
                {
                   firstId = input;
                }    
                enemyIdList.Add(input);
            }        
    }

    public void GenerateEnemies(int howManyEnemies, bool shouldSpawnSkeleton)
    {
        Vector3 enemyPosition = spawnerPos.position;
        if(shouldSpawnSkeleton)
        {
            enemyPosition = spawnerPos.position + new Vector3(5, 0, 0);
        }
        for (int i = 0; i < howManyEnemies; i++)
        {   
            GameObject enemyClone = Instantiate(enemyReference.gameObject, enemyPosition, Quaternion.identity);
            enemyClone.SetActive(true);
            if(shouldSpawnSkeleton)
            {
                enemyClone.GetComponent<EnemyScript>().EnemySetUp(1);
            }
            else
            {
                if (refereeScriptAccess.waveCount == 0)
                {
                    enemyClone.GetComponent<EnemyScript>().EnemySetUp(0);
                }
                else if (refereeScriptAccess.waveCount == 3)
                {
                    enemyClone.GetComponent<EnemyScript>().EnemySetUp(3);
                }
                else
                {
                    enemyClone.GetComponent<EnemyScript>().EnemySetUp(enemyIdList[i]);
                }
            }
            
            refereeScriptAccess.enemyList.Add(enemyClone.GetComponent<EnemyScript>());
            enemyPosition += new Vector3(5, 0, 0);                  
        }
        refereeScriptAccess.ResetChosenEnemy();
        refereeScriptAccess.RandomNumbersSetUpRoot();

    }
}
