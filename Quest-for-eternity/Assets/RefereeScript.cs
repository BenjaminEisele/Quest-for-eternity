using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RefereeScript : MonoBehaviour
{
    public EnemyScript targetEnemy;
    public PlayerStatScript playerAccess;

    public List<EnemyScript> enemyList;

    // [SerializeField]
    //TurnScript turnScriptAccess;
    private void Start()
    {
        enemyList.Add(targetEnemy);
    }
    public bool EnemyAttack()
    {
        foreach(EnemyScript enemy in enemyList)
        {
           // StartCoroutine(EnemyTurnCoroutine(enemy));
           
        }
        return true;
       // turnScriptAccess.StartPlayersTurn();
    }
    public void dealDamageToEnemy(int inputDamage)
    {
        targetEnemy.TakeDamage(inputDamage);


    }

    public void dealDamageToPlayer(int inputDamage)
    {
        playerAccess.TakeDamage(inputDamage);
        //playerAccess.playerHealth -= inputDamage;
    }

    private IEnumerator EnemyTurnCoroutine(EnemyScript enemy)
    {
        yield return new WaitForSeconds(3);
        dealDamageToPlayer(enemy.BeginAttack());
        Debug.Log("attack over");
    }
}
