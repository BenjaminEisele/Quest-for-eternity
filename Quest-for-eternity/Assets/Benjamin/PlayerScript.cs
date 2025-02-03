using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    HandScript handScriptAccess;
    [SerializeField]
    private Button EndTurnButton;
    [SerializeField]
    EnemyScript enemyScriptAccess;
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
    private int myPlayerListId;
    [SerializeField]
    ChooseNewCardScript chooseNewCardAccess;
    private bool shouldCheck = true;

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
                        RefereeScript.instance.hostId = RefereeScript.instance.playerList.Count - 1;
                        handScriptAccess.ActivateAllCardsEvent();
                    }
                    else 
                    {
                        isThisPlayersTurn = false;
                        isHost = false;
                        EndTurnButton.interactable = false;
                        RefereeScript.instance.clientId = RefereeScript.instance.playerList.Count - 1;
                    }
                }
                TurnScript.endTurnEvent += EndTurnPlayerScript;
                shouldCheck = false;
            }           
        }
    }

    public void EndTurnPlayerScript()
    {
        handScriptAccess.DisableAllCardsEvent();
        if (!isServer)
        {
            if (fieldScriptAccess.FieldHitCheck())
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                CmdDealDamage(damageThisRound, target);

            }
            Invoke("CmdEndTurn", 0.1f);
        }
        else if (isServer)
        {

            if (fieldScriptAccess.FieldHitCheck())
            {
                int target = RefereeScript.instance.chosenEnemyId;
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                RpcDealDamage(damageThisRound, target);
            }
            Invoke("RpcEndTurn", 0.1f);
        }
        handScriptAccess.UtlCardsPlayedForOtherPlayer = 0;
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
            this.EndTurnButton.interactable = isThisPlayersTurn;
            handScriptAccess.ActivateAllCardsEvent();
            RefereeScript.instance.isServersTurn = false;
        }
    }

    public void BeginPreNewWaveCall()
    {
        Debug.Log(transform.root.gameObject.name);
        if(isThisPlayersTurn)
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
