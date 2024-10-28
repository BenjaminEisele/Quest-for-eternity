using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatScript : MonoBehaviour
{
    public int playerHealth;

    TextMeshPro playerHealthText;
    private void Awake()
    {
        playerHealth = 25;
        playerHealthText = GetComponentInChildren<TextMeshPro>();
        playerHealthText.text = playerHealth.ToString();

    }
    public void TakeDamage(int inputDamage)
    {
        playerHealth -= inputDamage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("player is dead");
        }
        playerHealthText.text = playerHealth.ToString();
    }
}
