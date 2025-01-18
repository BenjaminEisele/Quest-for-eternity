using UnityEngine;
using TMPro;
using Mirror;

public class EnemyScript : NetworkBehaviour
{
    [SyncVar]
    public int enemyHealth;

    [SyncVar]
    public bool isEnemyAlive;

    public int personalId;

    int savedEnemyHealth;     
    TextMeshPro enemyHealthText;
    public GameObject myMarker;
    public DatabaseMultiplayer databaseMultiplayerAccess;


    public void EnemySetUp(int myID)
    {      
        personalId = myID;
        isEnemyAlive = true;
        this.enemyHealth = databaseMultiplayerAccess.enemyList[myID].enemyHealth;
        savedEnemyHealth = enemyHealth;
        enemyHealthText = GetComponentInChildren<TextMeshPro>();
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
    }
    public void ResetEnemy()
    {
        enemyHealth = savedEnemyHealth;
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
    }
    public void TakeDamageAndCheckIfDead(int inputDamage)
    {
        enemyHealth -= inputDamage;
        if(enemyHealth <= 0)
        {
            enemyHealth = 0;          
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
            isEnemyAlive = false;
            RefereeScript.instance.NewWaveCheck();
        }
        else
        {
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
            isEnemyAlive = true;
        }
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
            myDamage = databaseMultiplayerAccess.enemyList[personalId].GenerateAttack();          
        }
        else
        {
            myDamage = 0;
        }
        
        return myDamage;
    }
}
