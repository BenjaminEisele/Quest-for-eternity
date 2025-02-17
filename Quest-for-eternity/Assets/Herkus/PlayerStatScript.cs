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
    [SyncVar(hook = nameof(UpdateFighterTextInvocation))]
    public int playerArmor;
    int savedPlayerHealth;
    public List<int> immunityIdList;

    [SerializeField]
    TextMeshPro playerHealthText;
    [SerializeField]
    TextMeshPro playerArmorText;
    [SerializeField]
    PlayerScript playerScriptAccess;

    public int damageMultiplier;
    public int healingMultiplier;
    private void Start()
    {
        //playerScriptAccess = transform.root.GetComponentInChildren<PlayerScript>();
        Invoke("SubscriptionInvoke", 1f);
    }

    private void Awake()
    {
        playerHealth = 25;
        playerArmor = 0;
        damageMultiplier = 1;
        healingMultiplier = 1;
        savedPlayerHealth = playerHealth;
        //playerHealthText = GetComponentInChildren<TextMeshPro>();
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
        ResetPlayer();
        PlayerStatNewTurnEvent();
    }

    private void SubscriptionInvoke()
    {
        if(playerScriptAccess.isLocalGamePlayer)
        {
            if (isClientOnly)
            {
                RefereeScript.instance.newWaveEvent += ClientNewWaveHeal;
            }
            else
            {
                RefereeScript.instance.newWaveEvent += HostNewWaveHeal;
            }
            RefereeScript.instance.turnStartEvent += PlayerStatNewTurnEvent;
            RefereeScript.instance.newWaveEvent += PlayerStatNewWaveEvent;
        }
    }
    private void PlayerStatNewTurnEvent()
    {
        if(playerScriptAccess.isThisPlayersTurn)
        {
            damageMultiplier = 1;
            healingMultiplier = 1;
        }
    }

    public void ResetPlayerStatList()
    {
        immunityIdList.Clear();
    }
    private void PlayerStatNewWaveEvent()
    {
        playerArmor = 0;
    }
    public void ResetPlayer()
    {
        playerHealthOffset = 0;
        playerHealth = savedPlayerHealth;
        ChangePlayerHealth(savedPlayerHealth, 0);
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
    }

    private void ClientNewWaveHeal()
    {
        ChangeHealthNest(2, 0, true);
    }
    private void HostNewWaveHeal()
    {
        ChangeHealthNest(2, 0, false);
    }
    public void ChangeHealthNest(int input, int armorInput,bool shouldCallCommand)
    {
        if (isClientOnly)
        {
            if (shouldCallCommand)
            {
                CmdChangePlayerHealth(input, armorInput);
            }
        }
        else
        {
            ChangePlayerHealth(input, armorInput);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdChangePlayerHealth(int input, int armorInput)
    {
        ChangePlayerHealth(input, armorInput); 
    }

    public void ChangePlayerHealth(int desiredHealth, int desiredArmor)
    {
        playerArmor += desiredArmor;
        int damageDelta = 0;
        if(desiredHealth < 0)
        {
            damageDelta = playerArmor + desiredHealth;
            if(damageDelta < 0)
            {
                desiredHealth = damageDelta;
            }
            else
            {
                desiredHealth = 0;
            }
            
            playerArmor = damageDelta;
        }
        

        int changedValue = playerHealth + desiredHealth * healingMultiplier;
        playerHealth = changedValue;
        if(playerHealth >= savedPlayerHealth)
        {
            playerHealth = savedPlayerHealth;
        }
        if (playerArmor < 0)
        {
            playerArmor = 0;
        }
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage, int inputType)
    {
        if(!IsImmuneToAttack(inputType))
        {
            Debug.Log("didnt play knowledge card");
            inputDamage -= playerHealthOffset;
            if (inputDamage <= 0)
            {
                inputDamage = 0;
            }
            ChangeHealthNest(-inputDamage * damageMultiplier, 0, false);
            playerHealthOffset = 0;
        }
        else
        {
            Debug.Log("immune to attack?");
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

    public void CallMutation()
    {
        if(playerArmor > 0)
        {
            ChangeHealthNest(playerArmor, -playerArmor, true);
        }
        else
        {
            ChangeHealthNest(1, 0, true);
        }
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
        if(playerArmorText != null)
        {
            if(playerArmor > 0)
            {
                playerArmorText.color = Color.yellow;
                playerArmorText.gameObject.SetActive(true);
            }
            else
            {
                playerArmorText.gameObject.SetActive(false);
            }
            UiScript.UpdateFighterText(playerArmorText, playerArmor);
        }
    }
}
