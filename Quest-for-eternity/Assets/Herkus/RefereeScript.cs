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
    private void EndGame()
    {
        Debug.Log("game end");
        isGameOver = true;
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
    public void EnemyAttack()
    {
        if(!isGameOver)
        {
            foreach (EnemyScript enemy in enemyList)
            {
                StartCoroutine(EnemyTurnCoroutine(enemy));

            }
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
                EndGame();
            }
        }    
    }

    public void dealDamageToPlayer(int inputDamage)
    {
        if(playerAccess.TakeDamageAndCheckIfDead(inputDamage))
        {
            //turnScriptAccess.isPlayersTurn = false;
            turnScriptAccess.ShouldStartPlayerTurn(false);
            EndGame();
        }
        
        //playerAccess.playerHealth -= inputDamage;
    }

    private IEnumerator EnemyTurnCoroutine(EnemyScript enemy)
    {
        yield return new WaitForSeconds(0.75f);
        dealDamageToPlayer(enemy.BeginAttack());
        Debug.Log("attack over");
        UiScript.UpdateTurnInfo(0);
        turnScriptAccess.ShouldStartPlayerTurn(true);
    }
}
