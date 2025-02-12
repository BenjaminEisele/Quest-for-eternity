using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class PlayerStatScript : NetworkBehaviour
{
    public int playerHealthOffset;
    //[SyncVar(hook = nameof(UpdateFighterTextInvocation))]
    [SyncVar(hook = nameof(UpdateFighterTextInvocation))]
    public int playerHealth;
         
int savedPlayerHealth;
    //PlayerScript playerScriptAccess;

    TextMeshPro playerHealthText;
    private void Start()
    {
        Invoke("SubscriptionInvoke", 1f);
    }

    private void Awake()
    {
        
        //playerScriptAccess = transform.root.GetComponentInChildren<PlayerScript>();
        playerHealth = 25;
        savedPlayerHealth = playerHealth;
        playerHealthText = GetComponentInChildren<TextMeshPro>();
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
        ResetPlayer();
    }

    private void SubscriptionInvoke()
    {
        if(transform.root.GetComponentInChildren<PlayerScript>().isLocalGamePlayer)
        {
            if (isClientOnly)
            {
                RefereeScript.instance.newWaveEvent += ClientNewWaveHeal;
            }
            else
            {
                Debug.Log(transform.gameObject.name);
                RefereeScript.instance.newWaveEvent += HostNewWaveHeal;
            }
        }
       
    }
    public void ResetPlayer()
    {
        playerHealthOffset = 0;
        playerHealth = savedPlayerHealth;
        ChangePlayerHealth(savedPlayerHealth);
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
    }

    private void ClientNewWaveHeal()
    {
        ChangeHealthNest(2, true);
    }
    private void HostNewWaveHeal()
    {
        ChangeHealthNest(2, false);
    }
    public void ChangeHealthNest(int input, bool shouldCallCommand)
    {
        Debug.Log("Method call 0");
        if (isClientOnly)
        {
            if (shouldCallCommand)
            {
                CmdChangePlayerHealth(input);
                //UpdateFighterTextInvocation();
                //Invoke("UpdateFighterTextInvocation", 0.25f);
            }
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
        Debug.Log($"Desired amount is: {desiredAmount}");
        int changedValue = playerHealth + desiredAmount;
        playerHealth = changedValue;
        if(playerHealth < savedPlayerHealth)
        {
            //playerHealthText.color = Color.red;
        }
        else if(playerHealth >= savedPlayerHealth)
        {
            //playerHealthText.color = Color.white;
            playerHealth = savedPlayerHealth;
        }
        //UiScript.UpdateFighterText(playerHealthText, playerHealth);
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage)
    {
        inputDamage -= playerHealthOffset;
        if(inputDamage <= 0)
        {
            inputDamage = 0;
        }
        Debug.Log("Player took damage");
        ChangeHealthNest(-inputDamage, false);
        playerHealthOffset = 0;
        if (playerHealth <= 0)
        {
            Debug.Log("setting to 0");
            playerHealth = 0;
            //Invoke("UpdateFighterTextInvocation", 0.1f);
            //UiScript.UpdateFighterText(playerHealthText, playerHealth);
            return true;
        }
        else
        {
            //Invoke("UpdateFighterTextInvocation", 0.1f);
            return false;
        }       
    }
    public void UpdateFighterTextInvocation(int oldInt, int newInt)
    {
        if (playerHealthText != null)
        {
            Debug.Log($"Setting health to {playerHealth}");
            if (playerHealth < savedPlayerHealth)
            {

                playerHealthText.color = Color.red;
            }
            else
            {
                playerHealthText.color = Color.white;
            }
            UiScript.UpdateFighterText(playerHealthText, playerHealth);
        }
        
    }
}
