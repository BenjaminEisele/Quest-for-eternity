using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemyScript : MonoBehaviour
{
    //[Hide]
    public int enemyHealth;
    int savedEnemyHealth;
    int myId;
    public Database databaseAccess;

    public bool isEnemyAlive;
    TextMeshPro enemyHealthText;
    public TextMeshPro enemyNameText;
    public GameObject myMarker;

    public void EnemySetUp()
    {
        isEnemyAlive = true;
        myId = Random.Range(0, databaseAccess.enemyList.Count);
        this.enemyHealth = databaseAccess.enemyList[myId].enemyHealth;
        savedEnemyHealth = enemyHealth;
        // Debug.Log($"I am a {databaseAccess.enemyList[myId].enemyName}");


        //Debug.Log(GetComponentInChildren<SpriteRenderer>().gameObject.name);
        GetComponentInChildren<SpriteRenderer>().sprite = databaseAccess.enemyList[myId].enemySprite;
        enemyHealthText = GetComponentInChildren<TextMeshPro>();
        enemyNameText.text = databaseAccess.enemyList[myId].enemyName;
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
            myDamage = databaseAccess.enemyList[myId].GenerateAttack();
           // Debug.Log($"I dealt {myDamage} damage");
        }
        else
        {
            myDamage = 0;
            Debug.Log($"I can't attack because i am DEAD, my name is {databaseAccess.enemyList[myId].enemyName}");
        }
        
        return myDamage;
    }
}
