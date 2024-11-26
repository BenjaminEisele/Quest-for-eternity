using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatScript : MonoBehaviour
{
    public int playerHealth;
    int savedPlayerHealth;

    TextMeshPro playerHealthText;
    private void Awake()
    {
        playerHealth = 25;
        savedPlayerHealth = playerHealth;
        playerHealthText = GetComponentInChildren<TextMeshPro>();
        //playerHealthText.text = playerHealth.ToString();
        UiScript.UpdateFighterText(playerHealthText, playerHealth);

    }
    public void ResetPlayer()
    {
        playerHealth = savedPlayerHealth;
        //playerHealthText.text = playerHealth.ToString();
        ChangePlayerHealth(savedPlayerHealth);
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
       
    }

    public void ChangePlayerHealth(int desiredAmount)
    {
        //Debug.Log("health altered");
        int changedValue = playerHealth + desiredAmount;
        playerHealth = changedValue;
        if(playerHealth < savedPlayerHealth)
        {
            playerHealthText.color = Color.red;
        }
        else if(playerHealth >= savedPlayerHealth)
        {
            playerHealthText.color = Color.white;
            playerHealth = savedPlayerHealth;
            //Debug.Log("clamped");
        }
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage)
    {
        //playerHealth -= inputDamage;
        ChangePlayerHealth(-inputDamage);
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("player is dead");
            //UiScript.UpdateFighterText(playerHealthText, playerHealth);
            return true;
        }
        else
        {
            // playerHealthText.text = playerHealth.ToString();
            //UiScript.UpdateFighterText(playerHealthText, playerHealth);
            return false;
        }
    }
}
