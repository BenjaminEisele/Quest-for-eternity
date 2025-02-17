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
    public bool shouldDealDamage;
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
    private int myPlayerListId;
    [SerializeField]
    ChooseNewCardScript chooseNewCardAccess;
    private bool shouldCheck = true;
    public bool isLocalGamePlayer = false;

    public bool isPlayersTurnLocal;
    public bool shouldHealByDamageAmount;
    public int multiplier;
    public int healingSum;

    PlayerStatScript playerStatAccess;

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
                RefereeScript.instance.turnStartEvent += ResetHealingSum;
                RefereeScript.instance.turnStartEvent += SetLocalPlayersTurnTrue;
                turnScriptAccess.endTurnEvent += SetLocalPlayersTurnFalse;
                turnScriptAccess.endTurnEvent += DealDamageEventTrue;
                shouldCheck = false;
                isPlayerAlive = true;
                multiplier = 1;
            }           
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
        //Debug.Log("ending turn");
        handScriptAccess.DisableAllCardsEvent();
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
        if (isThisPlayersTurn)
        {
            RefereeScript.instance.playerList[0].DealDamageAsServer(inputDamage, target);
        }
    }

    [ClientRpc]
    public void DealDamageAsServer(int inputDamage2, int target)
    {
        if (RefereeScript.instance.enemyList.Count > 0)
        {
            Debug.Log($"target is: {target}");
            if (RefereeScript.instance.enemyList[target] != null)
            {
                healingSum = inputDamage2;
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
                Debug.Log($"target is: {target}");
                if (RefereeScript.instance.enemyList[target] != null)
                {
                    healingSum = inputDamage;
                    RefereeScript.instance.enemyList[target].TakeDamageAndCheckIfDead(inputDamage);
                }
            }
            
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        if (RefereeScript.instance.canTransferTurnToPlayer)
        {
            isThisPlayersTurn = !isThisPlayersTurn;
            turnScriptAccess.isPlayersTurn = isThisPlayersTurn;
            this.EndTurnButton.interactable = isThisPlayersTurn;
            handScriptAccess.ActivateAllCardsEvent();
            RefereeScript.instance.isServersTurn = true;
        }
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        if (RefereeScript.instance.canTransferTurnToPlayer)
        {
            damageThisRound = 0;
            isThisPlayersTurn = !isThisPlayersTurn;
            turnScriptAccess.isPlayersTurn = isThisPlayersTurn;
            this.EndTurnButton.interactable = isThisPlayersTurn;
            handScriptAccess.ActivateAllCardsEvent();
            RefereeScript.instance.isServersTurn = false;
        }
    }
    public void DealDamagePlayerScript(bool inputBool, bool shouldDealAoE, int setDamage, bool hammerEffect, bool activateDelayedEffecs)
    {
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
            if (fieldScriptAccess.CheckIfHitAndShouldClearField(inputBool, hasGuaranteedHit))
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid * multiplier;
                if(shouldDealAoE)
                {
                    for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                    {                      
                        CmdDealDamage(damageThisRound, j);
                        if(isLocalGamePlayer)
                        {
                            healingSum += damageThisRound;
                        }
                        
                    }
                }
                else
                {
                    CmdDealDamage(damageThisRound, target);
                    if (isLocalGamePlayer)
                    {
                        healingSum += damageThisRound;
                    }
                    Debug.Log($"Healing sum: {healingSum}");
                }
            }
            if(hammerEffect)
            {
                for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                {
                    CmdDealDamage(1, j);
                    if (isLocalGamePlayer)
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
            if (fieldScriptAccess.CheckIfHitAndShouldClearField(inputBool, hasGuaranteedHit))
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid * multiplier;
                if (shouldDealAoE)
                {
                    for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                    {
                        RpcDealDamage(damageThisRound, j);
                        if (isLocalGamePlayer)
                        {
                            healingSum += damageThisRound;
                        }
                    }
                }
                else
                {
                    RpcDealDamage(damageThisRound, target);
                    if (isLocalGamePlayer)
                    {
                        healingSum += damageThisRound;
                    }
                    Debug.Log($"Healing sum: {healingSum}");
                }
            }
            if (hammerEffect)
            {
                for (int j = 0; j < RefereeScript.instance.enemyList.Count; j++)
                {
                    RpcDealDamage(1, j);
                    if (isLocalGamePlayer)
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
        fieldScriptAccess.FieldClear();
        Debug.Log(transform.root.gameObject.name);
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
            Debug.Log("Pre new wave");
            RefereeScript.instance.playerList[0].isThisPlayersTurnToChoose = true;
            RefereeScript.instance.CallPreNewWaveEvent();
            EndTurnPlayerScript();
        }
        
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

    public void PlayCardForOtherPlayer(int cardID)
    {
        if (isServer)
        {
            RpcPlayCardForOtherPlayer(cardID);
        }

        else if (!isServer)
        {
            CmdPlayCardForOtherPlayer(cardID);
        }
    }

    [Command]
    private void CmdPlayCardForOtherPlayer(int cardID)
    {
       RefereeScript.instance.playerList[0].fieldScriptAccess.SpawnActiveCard(cardID, false);
    }

    [ClientRpc]
    private void RpcPlayCardForOtherPlayer(int cardID)
    {
        if (isClientOnly)
        {
            RefereeScript.instance.playerList[1].fieldScriptAccess.SpawnActiveCard(cardID, false);
        }
    }
}
