using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class EnemyScript : NetworkBehaviour
{
    [SyncVar]
    public int enemyHealth;

    [SyncVar]
    public bool isEnemyAlive;

    [SyncVar]
    public int myId;

    int savedEnemyHealth;     
    TextMeshPro enemyHealthText;
    public GameObject myMarker;
    public DatabaseMultiplayer databaseMultiplayerAccess;

    public int RandomNumber()
    {
        if (isServer)
        {
            Debug.Log("Test");
            myId = Random.Range(0, databaseMultiplayerAccess.enemyList.Count);
        }

        else { myId = 1; }
        return myId;
    }

    public void EnemySetUp()
    {
        isEnemyAlive = true;
        myId = RandomNumber();
        this.enemyHealth = databaseMultiplayerAccess.enemyList[myId].enemyHealth;
        savedEnemyHealth = enemyHealth;
       
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
            myDamage = databaseMultiplayerAccess.enemyList[myId].GenerateAttack();          
        }
        else
        {
            myDamage = 0;
        }
        
        return myDamage;
    }
}
