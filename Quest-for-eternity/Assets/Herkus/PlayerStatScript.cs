using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class PlayerStatScript : NetworkBehaviour
{
    public int playerHealthOffset;
    [SyncVar]
    public int playerHealth;
    int savedPlayerHealth;
    //PlayerScript playerScriptAccess;

    TextMeshPro playerHealthText;

    private void Awake()
    {
        //playerScriptAccess = transform.root.GetComponentInChildren<PlayerScript>();
        playerHealth = 25; //for testing and debugging
        savedPlayerHealth = playerHealth;
        playerHealthText = GetComponentInChildren<TextMeshPro>();
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
        ResetPlayer();
    }
    public void ResetPlayer()
    {
        playerHealthOffset = 0;
        playerHealth = savedPlayerHealth;
        ChangePlayerHealth(savedPlayerHealth);
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
    }

    public void ChangeHealthNest(int input)
    {
        if(isClientOnly)
        {
            CmdChangePlayerHealth(input);
        }
        else
        {
            ChangePlayerHealth(input);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdChangePlayerHealth(int input)
    {
        ChangePlayerHealth(input);
    }

    public void ChangePlayerHealth(int desiredAmount)
    {
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
        }
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage)
    {
        inputDamage -= playerHealthOffset;
        if(inputDamage <= 0)
        {
            inputDamage = 0;
        }
        ChangeHealthNest(-inputDamage);
        //ChangePlayerHealth(-inputDamage);
        playerHealthOffset = 0;
        if (playerHealth <= 0)
        {
            Debug.Log("setting to 0");
            playerHealth = 0;
            UiScript.UpdateFighterText(playerHealthText, playerHealth);

            return true;
        }
        else
        {
            UiScript.UpdateFighterText(playerHealthText, playerHealth);

            return false;
        }
        
    }
}
