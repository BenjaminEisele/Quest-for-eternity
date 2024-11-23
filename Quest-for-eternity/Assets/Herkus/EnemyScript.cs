using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemyScript : MonoBehaviour
{
    //[Hide]
    public int enemyHealth;
    int savedEnemyHealth;

    TextMeshPro enemyHealthText;

    //public bool isAlive;
    private void Awake()
    {
        enemyHealth = 25;
        savedEnemyHealth = enemyHealth;
        enemyHealthText = GetComponentInChildren<TextMeshPro>();
        //enemyHealthText.text = enemyHealth.ToString();
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
    }

    public void ResetEnemy()
    {
        enemyHealth = savedEnemyHealth;
        //enemyHealthText.text = enemyHealth.ToString();
        UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage)
    {
        enemyHealth -= inputDamage;
        if(enemyHealth <= 0)
        {
            enemyHealth = 0;
            Debug.Log("enemy is dead");
            //enemyHealthText.text = enemyHealth.ToString();
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
            return true;
        }
        else
        {
            //enemyHealthText.text = enemyHealth.ToString();
            UiScript.UpdateFighterText(enemyHealthText, enemyHealth);
            return false;
        }
    }

    public int GenerateAttack()
    {
        int enemyDamage = 3;//Random.Range(1, 6);
       // Debug.Log(enemyDamage);
        return enemyDamage;
    }
}
