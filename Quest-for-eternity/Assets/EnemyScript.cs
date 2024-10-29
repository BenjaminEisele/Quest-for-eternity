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
        enemyHealthText.text = enemyHealth.ToString();

    }

    public void ResetEnemy()
    {
        enemyHealth = savedEnemyHealth;
        enemyHealthText.text = enemyHealth.ToString();
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage)
    {
        enemyHealth -= inputDamage;
        if(enemyHealth <= 0)
        {
            enemyHealth = 0;
            Debug.Log("enemy is dead");
            enemyHealthText.text = enemyHealth.ToString();
            return true;
        }
        else
        {
            enemyHealthText.text = enemyHealth.ToString();
            return false;
        }
       
    }

    public int BeginAttack()
    {
        int enemyDamage = Random.Range(1, 6);
        Debug.Log(enemyDamage);
        return enemyDamage;
    }
}
