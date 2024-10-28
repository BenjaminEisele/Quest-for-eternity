using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemyScript : MonoBehaviour
{
    //[Hide]
    public int enemyHealth;

    TextMeshPro enemyHealthText;

    private void Awake()
    {
        enemyHealth = 25;
        enemyHealthText = GetComponentInChildren<TextMeshPro>();
        enemyHealthText.text = enemyHealth.ToString();

    }
    public void TakeDamage(int inputDamage)
    {
        enemyHealth -= inputDamage;
        if(enemyHealth <= 0)
        {
            enemyHealth = 0;
            Debug.Log("enemy is dead");
        }
        enemyHealthText.text = enemyHealth.ToString();
    }

    public int BeginAttack()
    {
        int enemyDamage = Random.Range(1, 6);
        Debug.Log(enemyDamage);
        return enemyDamage;
    }
}
