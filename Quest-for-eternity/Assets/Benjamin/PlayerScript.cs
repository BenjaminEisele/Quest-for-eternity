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
    [SerializeField]
    TurnScript turnScriptAccess;
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
    bool shouldDealDamageSingle = true;
    public bool isLocalGamePlayer = false;

    public bool isPlayersTurnLocal;

    public List<int> knowledgeIdList;

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
                turnScriptAccess.endTurnEvent += SetLocalPlayersTurnFalse;// EndTurnPlayerScript;
                turnScriptAccess.endTurnEvent += DealDamageEventTrue;
                shouldCheck = false;
                isPlayerAlive = true;
            }           
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            EndTurnPlayerScript();
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
        DealDamagePlayerScript(true);
    }
    public void EndTurnSubscription()
    {
        turnScriptAccess.endTurnEvent += EndTurnPlayerScript;
    }
    public void EndTurnPlayerScript()
    {
        handScriptAccess.DisableAllCardsEvent();
        if (!isServer)
        {
           /* if (fieldScriptAccess.CheckIfHitAndShouldClearField(true))
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                CmdDealDamage(damageThisRound, target);

            } */
            Invoke("CmdEndTurn", 0.1f);
        }
        else if (isServer)
        {
          /*  if (fieldScriptAccess.CheckIfHitAndShouldClearField(true))
            {
                if(shouldDealDamageSingle)
                {
                    int target = RefereeScript.instance.chosenEnemyId;
                    damageThisRound = fieldScriptAccess.damagePointsLiquid;
                    Debug.Log($"Damage is: {damageThisRound}");
                    RpcDealDamage(damageThisRound, target);
                }      
            } */
            Invoke("RpcEndTurn", 0.1f);
        }
        handScriptAccess.utlCardsPlayedForOtherPlayer = 0;
        shouldDealDamageSingle = true;
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
        RefereeScript.instance.enemyList[target].TakeDamageAndCheckIfDead(inputDamage2);
    }

    [ClientRpc]
    public void RpcDealDamage(int inputDamage, int target)
    {
        if (isThisPlayersTurn)
        {
            RefereeScript.instance.enemyList[target].TakeDamageAndCheckIfDead(inputDamage);
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
            if (!isPlayerAlive)
            {
                Debug.Log("Iam called to many times Cmd");
                //turnScriptAccess.CallEndTurnEvent();
            }
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
            if (!isPlayerAlive)
            {
                //turnScriptAccess.CallEndTurnEvent();
                Debug.Log("Iam called to many times Rpc");
            }
        }
    }
    public void DealDamagePlayerScript(bool inputBool)
    {
        bool hasGuaranteedHit = false;
        for(int i = 0; i < knowledgeIdList.Count; i++)
        {
            if (RefereeScript.instance.enemyList[RefereeScript.instance.chosenEnemyId].myEnemyType == knowledgeIdList[i])
            {
                hasGuaranteedHit = true;
                Debug.Log("guaranteed hit");
                break;
            }
        }       
        if (!isServer)
        {
            if (fieldScriptAccess.CheckIfHitAndShouldClearField(inputBool, hasGuaranteedHit))
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                CmdDealDamage(damageThisRound, target);
            }
        }
        else if (isServer)
        {
            if (fieldScriptAccess.CheckIfHitAndShouldClearField(inputBool, hasGuaranteedHit))
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                Debug.Log($"Damage is: {damageThisRound}");
                RpcDealDamage(damageThisRound, target);
            }
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
            shouldDealDamageSingle = false;
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
       RefereeScript.instance.playerList[0].fieldScriptAccess.SpawnActiveCard(cardID);
    }

    [ClientRpc]
    private void RpcPlayCardForOtherPlayer(int cardID)
    {
        if (isClientOnly)
        {
            RefereeScript.instance.playerList[1].fieldScriptAccess.SpawnActiveCard(cardID);
        }
    }
}
