using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
public class EnemyGenerator : NetworkBehaviour
{

    [SerializeField]
    EnemyScript enemyReference;

    [SerializeField]
    Transform spawnerPos;

    [SerializeField]
    RefereeScript refereeScriptAccess;

    [SerializeField]
    DatabaseMultiplayer databaseMultiplayerAccess;

    public List<int> difficultySumList;

    bool isEnoughSpaceLeft;
    int firstId;

    public int currentDifficultySum;

    public readonly SyncList<int> enemyIdList = new SyncList<int>();

    public void RandomNumber(int howMany)
    {     
        currentDifficultySum = 0;
        enemyIdList.Clear();
        int input;
        for(int i = 0; i < howMany; i++)
        {
            input = Random.Range(0, 3);
            while(!isValidEnemyDifficulty(input))
            {
                Debug.Log("false");
                input = Random.Range(0, 3);
            }
            currentDifficultySum += databaseMultiplayerAccess.enemyList[input].difficultyValue;
            if (i == 0)
            {
                firstId = input;
            }

            Debug.Log($"Id is: {input}");
            enemyIdList.Add(input);
            if (!IsEnoughSpaceLeft())
            {
                for(int j = 0; j < howMany - i - 1; j++)
                {
                    enemyIdList.Add(0);
                }
                break;
            }
        }
        Debug.Log($"Just before resetting, the difficulty sum was: {currentDifficultySum}");
        currentDifficultySum = 0;
    }

    private bool isValidEnemyDifficulty(int inputEnemyId)
    {
        Debug.Log($"Sum is: {databaseMultiplayerAccess.enemyList[inputEnemyId].difficultyValue + currentDifficultySum}");
        int waveIndex = RefereeScript.instance.waveCount;
       // if(waveIndex != 0)
        //{
        //    waveIndex--;
      //  }
        Debug.Log($"Comparing against: {difficultySumList[waveIndex]}");
        if (databaseMultiplayerAccess.enemyList[inputEnemyId].difficultyValue + currentDifficultySum > difficultySumList[waveIndex])
        {
            return false;
        }
        return true;
    }
    private bool IsEnoughSpaceLeft()
    {
        for(int i = 0; i < databaseMultiplayerAccess.enemyList.Count - 1; i++)
        {
            if(currentDifficultySum + databaseMultiplayerAccess.enemyList[i].difficultyValue <= difficultySumList[RefereeScript.instance.waveCount])
            {
                return true;
            }
        }
        return false;
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
                if (shouldSpawnSkeleton)
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
