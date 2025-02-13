using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Collections.Generic;

public class PlayerStatScript : NetworkBehaviour
{
    public int playerHealthOffset;
    [SyncVar(hook = nameof(UpdateFighterTextInvocation))]
    public int playerHealth;       
    int savedPlayerHealth;
    public List<int> immunityIdList;

    TextMeshPro playerHealthText;
    private void Start()
    {
        Invoke("SubscriptionInvoke", 1f);
    }

    private void Awake()
    {
        playerHealth = 25;
        savedPlayerHealth = playerHealth;
        playerHealthText = GetComponentInChildren<TextMeshPro>();
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
        ResetPlayer();
        ClearListEvent();
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
                RefereeScript.instance.newWaveEvent += HostNewWaveHeal;
            }
            RefereeScript.instance.turnStartEvent += ClearListEvent;
        }
    }
    private void ClearListEvent()
    {
        immunityIdList.Clear();
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
        if (isClientOnly)
        {
            if (shouldCallCommand)
            {
                CmdChangePlayerHealth(input);
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
        int changedValue = playerHealth + desiredAmount;
        playerHealth = changedValue;
        if(playerHealth >= savedPlayerHealth)
        {
            playerHealth = savedPlayerHealth;
        }
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage, int inputType)
    {
        if(!IsImmuneToAttack(inputType))
        {
            inputDamage -= playerHealthOffset;
            if (inputDamage <= 0)
            {
                inputDamage = 0;
            }
            ChangeHealthNest(-inputDamage, false);
            playerHealthOffset = 0;
        }
        if (playerHealth <= 0)
        {
            playerHealth = 0;

            return true;
        }
        else
        {
            return false;
        }       
    }
    private bool IsImmuneToAttack(int inputEnemyId)
    {
        for(int i = 0; i < immunityIdList.Count; i++)
        {
            if(inputEnemyId == immunityIdList[i])
            {
                return true;
            }
        }
        return false;
    }
    public void UpdateFighterTextInvocation(int oldInt, int newInt)
    {
        if (playerHealthText != null)
        {
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
