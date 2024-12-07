using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RefereeScript : MonoBehaviour
{
    [SerializeField]
    EnemyScript targetEnemy; //veliau noretusi padaryti, kad net nereiketu nieko tampyti per inspektoriu, kitaip sakant kad viskas po kapotu butu.

    public PlayerStatScript playerAccess;

    public List<EnemyScript> enemyList;

    [SerializeField]
    TurnScript turnScriptAccess;

    private bool isGameOver;

    public GameObject restartGameButton;
    public GameObject lostImage;
    public GameObject winImage;

    [SerializeField]
    int chosenEnemyId;
    private void Start()
    {
        //enemyList.Add(targetEnemy);
        isGameOver = false;
        restartGameButton.SetActive(false);
        winImage.SetActive(false);
        lostImage.SetActive(false);
        
        
        //ChooseNewEnemy(0);
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChooseNewEnemy(1);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChooseNewEnemy(-1);
        }
    }
    public void EnemyWaveSetup()
    {
        foreach(EnemyScript enemy in enemyList)
        {
            enemy.ChangeSelectedStatus(false);
        }
        chosenEnemyId = 0;
        targetEnemy = enemyList[chosenEnemyId];
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    private void ChooseNewEnemy(int inputDirection)
    {
        enemyList[chosenEnemyId].ChangeSelectedStatus(false);
        if (chosenEnemyId + inputDirection < enemyList.Count && chosenEnemyId + inputDirection >= 0)
        {
            chosenEnemyId += inputDirection;
        }
        else if(chosenEnemyId + inputDirection >= enemyList.Count)
        {
            chosenEnemyId = 0;
        }
        else if (chosenEnemyId + inputDirection < 0)
        {
            chosenEnemyId = enemyList.Count - 1;
        }


        targetEnemy = enemyList[chosenEnemyId];
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    private void EndGame(bool didPlayerWin)
    {
        turnScriptAccess.SetPlayerTurnBool(false);
        Debug.Log("game end");
        isGameOver = true;
        string winnerName;
        if(didPlayerWin)
        {
            winnerName = "The player";
            winImage.SetActive(true);
        }
        else
        {
            winnerName = "The enemy";
            lostImage.SetActive(true);
        }
        //UiScript.UpdateGameOverText($"Game over! {winnerName} is victorious!");
        restartGameButton.SetActive(true);
    }

    public void StartNextWave()
    {

    }
    public void RefereeReset()
    {
        isGameOver = false;
        foreach (EnemyScript enemy in enemyList)
        {
            enemy.ResetEnemy();
        }
        playerAccess.ResetPlayer();
    }
    public bool GetIsGameOver()
    {
        return isGameOver;
    }
    public void StartEnemyCoroutines()
    {
        if(!isGameOver)
        {
            StartCoroutine(EnemyTurnCoroutine());
            /* foreach (EnemyScript enemy in enemyList)
             {
                 StartCoroutine(EnemyTurnCoroutine(enemy));
             } 
             turnScriptAccess.ShouldStartPlayerTurn(true);*/
            Debug.Log("zzoz");//ar galima sitoj vietoj padaryti kad visa logika eitu tik per turn script puse?
        }
        
    }
    public void dealDamageToEnemy(int inputDamage)
    {
        //targetEnemy.TakeDamageAndCheckIfDead(inputDamage);
        //targetEnemy = enemyList[0];
        if (!targetEnemy.TakeDamageAndCheckIfDead(inputDamage))
        {
            bool areAllEnemiesDead = true;
            foreach (EnemyScript enemy in enemyList)
            {
                if(enemy.enemyHealth > 0)
                {
                    areAllEnemiesDead = false;
                }
            }
            if(areAllEnemiesDead)
            {
                StartNextWave();
               // EndGame(true);
            }
        }    
    }

    public void dealDamageToPlayer(int inputDamage)
    {
        if(playerAccess.TakeDamageAndCheckIfDead(inputDamage))
        {
            //turnScriptAccess.isPlayersTurn = false;
            turnScriptAccess.ShouldStartPlayerTurn(false);
            EndGame(false);
        }
        //playerAccess.playerHealth -= inputDamage;
    }

    private IEnumerator EnemyTurnCoroutine()//(EnemyScript enemy)
    {
        yield return new WaitForSeconds(0.25f);
        foreach (EnemyScript enemy in enemyList)
        {
            int enemyDamage = enemy.GenerateAttack();
            dealDamageToPlayer(enemyDamage);
            UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);
            yield return new WaitForSeconds(0.75f);
        }
        

        

        turnScriptAccess.ShouldStartPlayerTurn(true);
       // Debug.Log("attack over");
        //turnScriptAccess.ShouldStartPlayerTurn(true);
    }
}
