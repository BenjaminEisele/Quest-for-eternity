using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerStatScript : NetworkBehaviour
{
    public int playerHealthOffset;
    [SyncVar(hook = nameof(UpdateFighterTextInvocation))]
    public int playerHealth;
    [SyncVar(hook = nameof(UpdateFighterTextInvocation))]
    public int playerArmor;
    int savedPlayerHealth;
    public List<int> immunityIdList;
    public int immunityCount;

    public GameObject shieldIcon;

    [SerializeField]
    TextMeshPro playerHealthText;
    [SerializeField]
    TextMeshPro playerArmorText;
    [SerializeField]
    PlayerScript playerScriptAccess;

    [SerializeField] VoiceManager voiceManager;
    [SerializeField] UiScript uiScriptAccess;

    public int damageMultiplier;
    public int healingMultiplier;

    public GameObject damageIndicatorText;
    private void Start()
    {
        Invoke("SubscriptionInvoke", 1f);
    }

    private void Awake()
    {
        playerArmorText.gameObject.SetActive(false);
        playerHealth = 25;
        playerArmor = 0;
        damageMultiplier = 1;
        healingMultiplier = 1;
        savedPlayerHealth = playerHealth;
        UiScript.UpdateFighterText(playerHealthText, playerHealth);
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
            RefereeScript.instance.newWaveEvent += PlayerStatNewWaveEvent;
            RefereeScript.instance.restartGameEvent += ResetPlayerStat;
        }
    }

    public void ResetPlayerStatList()
    {
        immunityIdList.Clear();
    }
    private void PlayerStatNewWaveEvent()
    {
        playerArmor = 0;
        immunityIdList.Clear();
        immunityCount = 0;
        damageMultiplier = 1;
        healingMultiplier = 1;
        playerHealthOffset = 0;
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
                CmdChangePlayerHealth(input, armorInput, healingMultiplier);
            }
        }
        else
        {
            ChangePlayerHealth(input, armorInput, healingMultiplier);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdChangePlayerHealth(int input, int armorInput, int multiplierInput)
    {
        ChangePlayerHealth(input, armorInput, multiplierInput); 
    }

    public void ChangePlayerHealth(int desiredHealth, int desiredArmor, int multiplierInput)
    {
        playerArmor += desiredArmor;
        int damageDelta = 0;
        if(desiredArmor >= 0)
        {
            if (desiredHealth < 0)
            {
                multiplierInput = 1;
                damageDelta = playerArmor + desiredHealth;
                if (damageDelta < 0)
                {
                    desiredHealth = damageDelta;
                }
                else
                {
                    desiredHealth = 0;
                }

                playerArmor = damageDelta;
            }
        }
        else
        {
            Debug.Log("this doesnt get executed");
        }
       
        int changedValue = playerHealth + desiredHealth * multiplierInput;
        if (multiplierInput != 1)
        {
            healingMultiplier = 1;
            uiScriptAccess.DestroyIcon(27, 0);
        }
        playerHealth = changedValue;
        if (playerHealth >= savedPlayerHealth)
        {
            playerHealth = savedPlayerHealth;
        }
        else if(playerHealth <= 0)
        {
            playerScriptAccess.isPlayerAlive = false;
            playerHealth = 0;
        }
        if (playerHealth > 0)
        {
            playerScriptAccess.isPlayerAlive = true;
        }
        if (playerArmor < 0)
        {
            playerArmor = 0;
        }
    }
    public bool TakeDamageAndCheckIfDead(int inputDamage, int inputType, int voiceReference)
    {
        Debug.Log($"Im being Attacked my name is: {transform.root.gameObject.name}");
        if(!IsImmuneToAttack(inputType))
        {
            DamageIndicatorTween(inputDamage.ToString());
            //damageIndicatorText.text = inputDamage.ToString();
            inputDamage -= playerHealthOffset;
            if(inputDamage <= 0)
            {
                inputDamage = 0;
            }
            ChangeHealthNest(-inputDamage * damageMultiplier, 0, true);
            playerHealthOffset = 0;
            //Debug.Log()
            uiScriptAccess.DestroyIcon(15, 0);
            uiScriptAccess.DestroyIcon(43, 0);

        }
        else
        {
            DamageIndicatorTween("0");

            // damageIndicatorText.text = "0";
            immunityCount--;
            if(immunityCount <= 0)
            {
                uiScriptAccess.DestroyIcon(10,1);
                uiScriptAccess.DestroyIcon(22, 0);
                immunityIdList.Clear();
            }
        }
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            voiceManager.KillPlayerLine(voiceReference);
            
            return true;
        }
        else
        {
            Debug.Log($"Health is: {playerHealth}");
            return false;
        }       
    }

    private void DamageIndicatorTween(string inputString)
    {
        GameObject attackIndicatorClone = Instantiate(damageIndicatorText, playerHealthText.gameObject.transform.position, Quaternion.identity);
        attackIndicatorClone.SetActive(true);
        attackIndicatorClone.GetComponent<TextMeshPro>().text = inputString;
        attackIndicatorClone.transform.DOMoveY(2, 2);
        attackIndicatorClone.GetComponent<TextMeshPro>().color = Color.red;
        //Color newColor = Color.red;
        Color newColor = new Color(1, 0, 0, 0);
        attackIndicatorClone.GetComponent<TextMeshPro>().DOColor(newColor, 2);
        StartCoroutine(DestructionCoroutine(attackIndicatorClone));
    }
    
    private IEnumerator DestructionCoroutine(GameObject inputGameObj)
    {
        yield return new WaitForSeconds(3);
        Destroy(inputGameObj);
    }
    private bool IsImmuneToAttack(int inputEnemyId)
    {
        for(int i = 0; i < immunityIdList.Count; i++)
        {
            Debug.Log("for");
            if(inputEnemyId == immunityIdList[i])
            {
                Debug.Log("immune");
                return true;
            }
        }
        return false;
    }

    public void CallMutation()
    {
        if(playerArmor > 0)
        {
            ChangeHealthNest(playerArmor * 2, -playerArmor, true);
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
                playerHealthText.color = Color.grey;
            }
            UiScript.UpdateFighterText(playerHealthText, playerHealth);
        }
        if(playerArmorText != null)
        {
            if(playerArmor > 0)
            {
                playerArmorText.color = Color.grey;
                playerArmorText.gameObject.SetActive(true);
                if (shieldIcon != null)
                {
                    shieldIcon.SetActive(true);
                }
            }
            else
            {
                if(shieldIcon != null)
                {
                    shieldIcon.SetActive(false);
                }
                playerArmorText.gameObject.SetActive(false);
            }
            UiScript.UpdateFighterText(playerArmorText, playerArmor);
        }       
    }

    private void ResetPlayerStat()
    {
        Debug.Log("Reset player");
        ChangeHealthNest(50, -10000, true);
        immunityIdList.Clear();
        immunityCount = 0;
        damageMultiplier = 1;
        healingMultiplier = 1;
        playerHealthOffset = 0;
    }
}
