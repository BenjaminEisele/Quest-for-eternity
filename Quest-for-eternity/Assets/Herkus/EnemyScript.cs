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
    public TextMeshPro enemyNameText; // public temporarily
    [SerializeField]
    GameObject myMarker;
    [SerializeField]
    DatabaseMultiplayer databaseMultiplayerAccess;
    [SerializeField]
    EnemyGenerator enemyGeneratorAccess;



    public bool canAttack;


    

    public void EnemySetUp(int myID)
    {
        canAttack = true;
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
        databaseMultiplayerAccess.updatedLootList.Add(databaseMultiplayerAccess.enemyList[personalId].lootCardId);
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
        if (enemyHealth >= savedEnemyHealth)
        {
            enemyHealth = savedEnemyHealth;
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
        }
        if (enemyHealth <= 0)
        {
            enemyHealth = 0;          
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
            isEnemyAlive = false;
            RefereeScript.instance.enemyList.Remove(this);
            RefereeScript.instance.killedEnemyList.Add(this);
            gameObject.SetActive(false);
            RefereeScript.instance.NewWaveCheck();
            RefereeScript.instance.ResetChosenEnemy();
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
    private void EnemySpawnLogic()
    {
        specialAttackCounter = 0;
        RefereeScript.instance.SpecialAttackCounterNest(true);
        if (RefereeScript.instance.enemyList.Count >= 2)
        {
            Debug.Log($"{personalId} healing enemy");
            RefereeScript.instance.HealEnemyRefereeScript();
        }
        else
        {
            Debug.Log($"{personalId} spawning enemy");
            RefereeScript.instance.EnemyGenerationNest();
        }
    }
    public int GenerateAttack()
    {
       int myDamage;
       

        if (isEnemyAlive)
        {
            if (specialAttackCounter >= 2)
            {
                EnemySpawnLogic();
                myDamage = 0;
            }
            else
            {
                if (isBoss)
                {
                    specialAttackCounter++;
                    RefereeScript.instance.SpecialAttackCounterNest(false);
                }
                myDamage = databaseMultiplayerAccess.enemyList[personalId].GenerateAttack();
            }
        }
        else
        {
           myDamage = 0;
        }
        return myDamage;
    }

}
