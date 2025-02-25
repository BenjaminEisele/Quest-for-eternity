using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    HandScript handScriptAccess;
    [SerializeField]
    private Button EndTurnButton;
    public TurnScript turnScriptAccess;
    [SerializeField]
    FieldScript fieldScriptAccess;
    [SyncVar]
    public bool isHost;
    [SyncVar]
    public bool isThisPlayersTurn;
    [SyncVar]
    public bool isThisPlayersTurnToChoose;
    [SyncVar] 
    public int damageThisRound;
    [SyncVar]
    public bool isPlayerAlive;
    [SerializeField]
    ChooseNewCardScript chooseNewCardAccess;
    private bool shouldCheck = true;
    public bool isLocalGamePlayer = false;

    public bool isPlayersTurnLocal;
    public bool shouldHealByDamageAmount;
    public int multiplier;
    public int healingSum;
    public bool areaAttackActive;

    [SerializeField] VoiceManager voiceManager;

    private PlayerStatScript playerStatAccess;
    public List<int> knowledgeIdList;

    private void Start()
    {
        isPlayerAlive = true;
        playerStatAccess = transform.root.GetComponentInChildren<PlayerStatScript>();
    }

    public void Update()
    {
        if (shouldCheck)
        {
            if (RefereeScript.instance != null)
            {
                if (isOwned)
                {
                    if (isServer)
                    {
                        isThisPlayersTurn = true;
                        isHost = true;

                        EndTurnButton.interactable = true;
                        handScriptAccess.ActivateAllCardsEvent();
                        isPlayersTurnLocal = true;
                    }
                    else 
                    {
                        isThisPlayersTurn = false;
                        isHost = false;
                        EndTurnButton.interactable = false;
                    }
                }
                RefereeScript.instance.turnStartEvent += EndTurnPlayerScript;
                RefereeScript.instance.turnStartEvent += SetLocalPlayersTurnTrue;
                RefereeScript.instance.turnStartEvent += ResetHealingSum;
                RefereeScript.instance.restartGameEvent += RestartPlayerScript;
                turnScriptAccess.endTurnEvent += SetLocalPlayersTurnFalse;
                turnScriptAccess.endTurnEvent += DealDamageEventTrue;
                shouldCheck = false;
                isPlayerAlive = true;
                multiplier = 1;
            }           
        }
    }

    private void RestartPlayerScript()
    {
        Invoke("RestartPlayerScriptLogic", 0.2f);   
    }

    private void RestartPlayerScriptLogic()
    {
        isPlayerAlive = true;
        shouldHealByDamageAmount = false;
        multiplier = 1;
        healingSum = 0;
        areaAttackActive = false;
        knowledgeIdList.Clear();
        if (isHost)
        {
            isThisPlayersTurn = true;
            isPlayersTurnLocal = true;
            handScriptAccess.SetCardActivityStatus(true, 2);
        }
        else
        {
            isThisPlayersTurn = false;
            isPlayersTurnLocal = false;
            handScriptAccess.SetCardActivityStatus(false, 2);
        }
    }

    private void SetLocalPlayersTurnFalse()
    {
        isPlayersTurnLocal = false;
    }
    private void SetLocalPlayersTurnTrue()
    {
        //isPlayersTurnLocal = true;
    }
    private void DealDamageEventTrue()
    {
        DealDamagePlayerScript(true, false, 0, false, true);
    }
    public void EndTurnSubscription()
    {
        turnScriptAccess.endTurnEvent += EndTurnPlayerScript;
    }
    public void EndTurnPlayerScript()
    {
        handScriptAccess.DisableAllCardsEvent();
        isThisPlayersTurn = false;
        turnScriptAccess.isPlayersTurn = false;
        EndTurnButton.interactable = false;
        if (!isServer)
        {
            Invoke("CmdEndTurn", 0.1f);
        }
        else if (isServer)
        {
            Invoke("RpcEndTurn", 0.1f);
        }
        handScriptAccess.utlCardsPlayedForOtherPlayer = 0;
        knowledgeIdList.Clear();
    }
    
    [Command(requiresAuthority = false)]
    public void CmdDealDamage(int inputDamage, int target)
    {
        if (!isThisPlayersTurn)
        {
            RefereeScript.instance.playerList[0].DealDamageAsServer(inputDamage, target);
        }
    }

    [ClientRpc]
    public void DealDamageAsServer(int inputDamage2, int target)
    {
        if (RefereeScript.instance.enemyList.Count > 0)
        {
            if ((target + 1 > RefereeScript.instance.enemyList.Count) && target != 0)
            {
                target--;
            }
            if (RefereeScript.instance.enemyList[target] != null)
            {
                RefereeScript.instance.enemyList[target].TakeDamageAndCheckIfDead(inputDamage2);
            }
        }
    }

    [ClientRpc]
    public void RpcDealDamage(int inputDamage, int target)
    {
        if (isThisPlayersTurn)
        {
            if(RefereeScript.instance.enemyList.Count > 0)
            {
                if ((target + 1 > RefereeScript.instance.enemyList.Count) && target != 0)
                {
                    target--;
                }
                if (RefereeScript.instance.enemyList[target] != null)
                {
                    RefereeScript.instance.enemyList[target].TakeDamageAndCheckIfDead(inputDamage);
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        if (isServer && isLocalPlayer)
        {
            if (RefereeScript.instance.canTransferTurnToPlayer)
            {
                isThisPlayersTurn = true;
                turnScriptAccess.isPlayersTurn = true;
                EndTurnButton.interactable = true;
                handScriptAccess.ActivateAllCardsEvent();
                RefereeScript.instance.isServersTurn = true;
            }
        }
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        if (isClientOnly && isLocalPlayer)
        {
            if (RefereeScript.instance.canTransferTurnToPlayer)
            {
                damageThisRound = 0;
                isThisPlayersTurn = true;
                turnScriptAccess.isPlayersTurn = true;
                EndTurnButton.interactable = true;
                handScriptAccess.ActivateAllCardsEvent();
                RefereeScript.instance.isServersTurn = false;
            }
        }
    }
    public void DealDamagePlayerScript(bool inputBool, bool shouldDealAoE, int setDamage, bool hammerEffect, bool activateDelayedEffecs)
    {
        Debug.Log("DealDamage");
        bool hasGuaranteedHit = false;
        for(int i = 0; i < knowledgeIdList.Count; i++)
        {
            if (RefereeScript.instance.enemyList[RefereeScript.instance.chosenEnemyId].myEnemyType == knowledgeIdList[i])
            {
                hasGuaranteedHit = true;
                break;
            }
        }       
        if (!isServer)
        {
            // 0 - Hit
            // 1 - Miss
            // 2 - no action card
            int hitOutcome = fieldScriptAccess.CheckIfHitAndShouldClearField(inputBool, hasGuaranteedHit);
            if (hitOutcome == 0)
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid * multiplier;
                multiplier = 1;
                transform.root.GetComponentInChildren<UiScript>().DestroyIcon(26, 0);
                if (damageThisRound > 4 && (Random.Range(1, 101) > 101))
                {
                    voiceManager.StrongAttackLine();
                }

                if(areaAttackActive)
                {
                    shouldDealAoE = true;
                    areaAttackActive = false;
                }
                if(shouldDealAoE)
                {
                    for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                    {                      
                        CmdDealDamage(damageThisRound, j);
                        if(isThisPlayersTurn)
                        {
                            healingSum += damageThisRound;
                        }                       
                    }
                }
                else
                {
                    CmdDealDamage(damageThisRound, target);
                    if (isThisPlayersTurn)
                    {
                        healingSum = damageThisRound;
                    }
                }
            }
            else if(hitOutcome == 1)
            {
                multiplier = 1;
                transform.root.GetComponentInChildren<UiScript>().DestroyIcon(26, 0);
            }
            
            if (hammerEffect)
            {
                for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                {
                    CmdDealDamage(1, j);
                    if (isThisPlayersTurn)
                    {
                        healingSum += 1;
                    }
                }
            }
            if(activateDelayedEffecs)
            {
                handScriptAccess.DelayedActionCardEffectActivation();
            }

            if (shouldHealByDamageAmount)
            {
                playerStatAccess.ChangeHealthNest(healingSum, 0, true);
                shouldHealByDamageAmount = false;
            }
        }
        else if (isServer)
        {
            int hitOutcome = fieldScriptAccess.CheckIfHitAndShouldClearField(inputBool, hasGuaranteedHit);
            if (hitOutcome == 0)
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid * multiplier;
                transform.root.GetComponentInChildren<UiScript>().DestroyIcon(26, 0);
                if (damageThisRound > 3 && (Random.Range(0f, 1f) > 0.33))
                {
                   // voiceManager.StrongAttackLine();
                }

                if (areaAttackActive)
                {
                    shouldDealAoE = true;
                    areaAttackActive = false;
                }
                if (shouldDealAoE)
                {
                    for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                    {
                        RpcDealDamage(damageThisRound, j);
                        if (isThisPlayersTurn)
                        {
                            healingSum += damageThisRound;
                        }
                    }
                }
                else
                {
                    RpcDealDamage(damageThisRound, target);
                    if (isThisPlayersTurn)
                    {
                        healingSum = damageThisRound;
                    }
                }
            }
            else if (hitOutcome == 1)
            {
                multiplier = 1;
                transform.root.GetComponentInChildren<UiScript>().DestroyIcon(26, 0);
            }

            if (hammerEffect)
            {
                for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                {
                    RpcDealDamage(1, j);
                    if (isThisPlayersTurn)
                    {
                        healingSum += 1;
                    }
                }
            }
            if (activateDelayedEffecs)
            {
                handScriptAccess.DelayedActionCardEffectActivation();
            }
            if(shouldHealByDamageAmount)
            {
                playerStatAccess.ChangeHealthNest(healingSum,0, false);
                shouldHealByDamageAmount = false;
            }
        }
    }
    public void GenerateProvocationIcon()
    {
        transform.root.GetComponentInChildren<UiScript>().DestroyIcon(29, 0);

    }
    private void ResetHealingSum()
    {
        if(isLocalGamePlayer)
        {
            healingSum = 0;
        }
    }

    public void HealEnemyPlayerScript()
    {
        int damage = -3;
        int target = 1;
        if (isServer)
        {
            RpcDealDamage(damage, target);
        }
        else
        {
            CmdDealDamage(damage, target);
        }
    }

    public void BeginPreNewWaveCall()
    {
        handScriptAccess.DisableAllCardsEvent();
        ClearFieldNest();
        if(!RefereeScript.instance.singlePlayerMode)
        {
            if (isThisPlayersTurn)
            {
                if (isHost && isServer)
                {
                    RefereeScript.instance.playerList[0].isThisPlayersTurnToChoose = true;
                    RefereeScript.instance.playerList[1].isThisPlayersTurnToChoose = false;
                    RefereeScript.instance.CallPreNewWaveEvent();
                    CallNewCardsAsServer();
                }
                else if (isClientOnly)
                {
                    CmdPreNewWaveCall();
                }
            }
        }
        else
        {
            RefereeScript.instance.playerList[0].isThisPlayersTurnToChoose = true;
            RefereeScript.instance.CallPreNewWaveEvent();
            EndTurnPlayerScript();
        }
        
    }

    private void ClearFieldNest()
    {
        if (isClientOnly)
        {
            CmdClearField();
        }
        else
        {
            RpcClearField();
        }
    }

    [ClientRpc]
    private void RpcClearField()
    {
        if (isLocalPlayer)
        {
            fieldScriptAccess.FieldClear();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdClearField()
    {
        RpcClearField();
    }

    [Command(requiresAuthority = false)]
    private void CmdPreNewWaveCall()
    {
        if (!isHost)
        {
            RefereeScript.instance.playerList[0].isThisPlayersTurnToChoose = false;
            RefereeScript.instance.playerList[1].isThisPlayersTurnToChoose = true;

            RefereeScript.instance.CallPreNewWaveEvent();
            CallNewCardsAsServer();
        }
    }

    [ClientRpc]
    public void CallNewCardsAsServer()
    {
        if (isClientOnly)
        {
            RefereeScript.instance.playerList[1].chooseNewCardAccess.DisplayCards();
        }
    }

    public void BeginDisplayCardSynchronization(int inputId)
    {
        isThisPlayersTurnToChoose = false;
        if (!isServer)
        {
            CmdSyncrhonizeCardDestruction(inputId);
        }
        else if (isServer)
        {
            DestroyCardAsServer(inputId);
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSyncrhonizeCardDestruction(int inputId)
    {
        DestroyCardAsClient(inputId);
    }

    [ClientRpc]
    public void DestroyCardAsClient(int inputId)
    {
        RefereeScript.instance.playerList[0].chooseNewCardAccess.FindAndDestroyCard(inputId);
    }

    [ClientRpc]
    public void DestroyCardAsServer(int inputId)
    {       
       RefereeScript.instance.playerList[1].chooseNewCardAccess.FindAndDestroyCard(inputId);
    }

    [Command(requiresAuthority = false)]
    public void DisplayCardsCallNest()
    {
        RefereeScript.instance.playerList[0].DisplayCardsCall();
    }

    [ClientRpc]
    public void DisplayCardsCall()
    {
        if (isClientOnly)
        {
            RefereeScript.instance.playerList[1].transform.root.GetComponentInChildren<ChooseNewCardScript>().DisplayCardsHidden();
        }
    }

    public void PlayCardForOtherPlayer(int cardID, bool playForOtherPlayer)
    {
        if (isServer)
        {
            RpcPlayCardForOtherPlayer(cardID, playForOtherPlayer);
        }

        else if (!isServer)
        {
            CmdPlayCardForOtherPlayer(cardID, playForOtherPlayer);
        }
    }

    [Command]
    private void CmdPlayCardForOtherPlayer(int cardID, bool playForOtherPlayer)
    {
       RefereeScript.instance.playerList[0].fieldScriptAccess.SpawnActiveCard(cardID, false, playForOtherPlayer);
    }

    [ClientRpc]
    private void RpcPlayCardForOtherPlayer(int cardID, bool playForOtherPlayer)
    {
        if (isClientOnly)
        {
            RefereeScript.instance.playerList[1].fieldScriptAccess.SpawnActiveCard(cardID, false, playForOtherPlayer);
        }
    }
}
