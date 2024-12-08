using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class EnemyScript : NetworkBehaviour
{
    //[Hide]
    [SyncVar]
    public int enemyHealth;
    int savedEnemyHealth;
    int myId;
    //public Database databaseAccess;

    [SyncVar]
    public bool isEnemyAlive;
    TextMeshPro enemyHealthText;
    public GameObject myMarker;

    public void EnemySetUp()
    {
        isEnemyAlive = true;
        myId = Random.Range(0, Database.instance.enemyList.Count);
        this.enemyHealth = Database.instance.enemyList[myId].enemyHealth;
        savedEnemyHealth = enemyHealth;
       // Debug.Log($"I am a {databaseAccess.enemyList[myId].enemyName}");

        
        enemyHealthText = GetComponentInChildren<TextMeshPro>();
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
    }
    public void ResetEnemy()
    {
        enemyHealth = savedEnemyHealth;
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage)
    {
        enemyHealth -= inputDamage;
        if(enemyHealth <= 0)
        {
            enemyHealth = 0;
           // Debug.Log("enemy is dead");
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
            isEnemyAlive = false;
        }
        else
        {
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
            isEnemyAlive = true;
        }
        return isEnemyAlive;
    }
    public void ChangeSelectedStatus(bool inputBool)
    {
        myMarker.SetActive(inputBool);
    }
    public int GenerateAttack()
    {
        int myDamage;
        if(isEnemyAlive)
        {
            myDamage = Database.instance.enemyList[myId].GenerateAttack();
           // Debug.Log($"I dealt {myDamage} damage");
        }
        else
        {
            myDamage = 0;
            Debug.Log($"I can't attack because i am DEAD, my name is {Database.instance.enemyList[myId].enemyName}");
        }
        
        return myDamage;
    }
}
