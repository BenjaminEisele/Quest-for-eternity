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
    private void Start()
    {
        enemyList.Add(targetEnemy);
        isGameOver = false;
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
        }
        else
        {
            winnerName = "The enemy";
        }
        UiScript.UpdateGameOverText($"Game over! {winnerName} is victorious!");
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
            foreach (EnemyScript enemy in enemyList)
            {
                StartCoroutine(EnemyTurnCoroutine(enemy));
            }
            turnScriptAccess.ShouldStartPlayerTurn(true);
            Debug.Log("zzoz");//ar galima sitoj vietoj padaryti kad visa logika eitu tik per turn script puse?
        }
        
    }
    public void dealDamageToEnemy(int inputDamage)
    {
        //targetEnemy.TakeDamageAndCheckIfDead(inputDamage);

        if (targetEnemy.TakeDamageAndCheckIfDead(inputDamage))
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
                EndGame(true);
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

    private IEnumerator EnemyTurnCoroutine(EnemyScript enemy)
    {
        yield return new WaitForSeconds(0.75f);

        int enemyDamage = enemy.GenerateAttack();
        dealDamageToPlayer(enemyDamage);
        UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);


        Debug.Log("attack over");
        //turnScriptAccess.ShouldStartPlayerTurn(true);
    }
}
