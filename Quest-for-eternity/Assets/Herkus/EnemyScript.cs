using UnityEngine;
using TMPro;
using Mirror;

public class EnemyScript : NetworkBehaviour
{
    [SyncVar]
    public int specialAttackCounter;
    [SyncVar]
    public int enemyHealth;
    public int personalId;
    int savedEnemyHealth;


    [SyncVar]
    public bool isEnemyAlive;
    public bool isBoss;


    TextMeshPro enemyHealthText;
    [SerializeField]
    TextMeshPro enemyNameText;
    [SerializeField]
    GameObject myMarker;
    [SerializeField]
    DatabaseMultiplayer databaseMultiplayerAccess;

    

    

    public void EnemySetUp(int myID)
    {
        specialAttackCounter = 0;
        personalId = myID;
        isEnemyAlive = true;
        this.enemyHealth = databaseMultiplayerAccess.enemyList[myID].enemyHealth;
        savedEnemyHealth = enemyHealth;
        this.isBoss = databaseMultiplayerAccess.enemyList[myID].isBoss;
        GetComponentInChildren<SpriteRenderer>().sprite = databaseMultiplayerAccess.enemyList[personalId].enemySprite;
        enemyHealthText = GetComponentInChildren<TextMeshPro>();
        enemyNameText.text = databaseMultiplayerAccess.enemyList[personalId].enemyName;
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);  
    }
    public void ResetEnemy()
    {
        enemyHealth = savedEnemyHealth;
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
    }
    public void TakeDamageAndCheckIfDead(int inputDamage)
    {
        Debug.Log("taking damage");
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
       if (isEnemyAlive)
       {
           myDamage = databaseMultiplayerAccess.enemyList[personalId].GenerateAttack();
       }
       else
       {
           myDamage = 0;
       }
       specialAttackCounter++;
       return myDamage;
    }
}
