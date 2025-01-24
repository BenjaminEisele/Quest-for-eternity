using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    private HandScript handScriptAccess;
    public Button EndTurnButton;
    public EnemyScript enemyScriptAccess;
    [SerializeField]
    FieldScript fieldScriptAccess;
    public bool shouldDealDamage;

    [SyncVar]
    public bool isHost;
    [SyncVar]
    public bool isThisPlayersTurn;

    [SyncVar]
    public bool isThisPlayersTurnToChoose;

    [SyncVar] public int damageThisRound;

    public int myPlayerListId;

    [SerializeField]
    ChooseNewCardScript chooseNewCardAccess;


    public void Start()
    {
        RefereeScript.instance.playerList.Add(this);
        isThisPlayersTurnToChoose = true;

        TurnScript.endTurnEvent += EndTurnPlayerScript;

        if (isOwned)
        {
            if (isServer)
            {
                // Debug.Log("I am the server");
                isThisPlayersTurn = true;
                isHost = true;
                EndTurnButton.interactable = true;
                RefereeScript.instance.hostId = RefereeScript.instance.playerList.Count - 1;
            }
            else //(!isServer)
            {
                // Debug.Log("I am NOT the server");
                isThisPlayersTurn = false;
                isHost = false;
                EndTurnButton.interactable = false;
                RefereeScript.instance.clientId = RefereeScript.instance.playerList.Count - 1;
            }
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if(isHost)
            {
                isThisPlayersTurnToChoose = true;
                RefereeScript.instance.CallPreNewWaveEvent();
                CallNewCardsAsServer();
            }
        } 
    }

    public void BeginPreNewWaveCall()
    {
        isThisPlayersTurnToChoose = true;
        if (isServer)
        {
            Debug.Log("pre new wave called as the server");
            RefereeScript.instance.CallPreNewWaveEvent();
            CallNewCardsAsServer();
        }
        else if(!isServer)
        {
            CmdPreNewWaveCall();
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdPreNewWaveCall()
    {
        Debug.Log("Called cmd pre new wave");
        RefereeScript.instance.CallPreNewWaveEvent();
        CallNewCardsAsServer();
    }

    public void BeginDisplayCardSynchronization()
    {
        isThisPlayersTurnToChoose = false;
        if (!isServer)
        {
            CmdSyncrhonizeCardDestruction();
        }
        else if (isServer)
        {
            DestroyCardAsServer();
        }
    }

    [ClientRpc]
    public void CallNewCardsAsServer()
    {
        Debug.Log("Card hopefully added");
        if(isClientOnly)
        {
            RefereeScript.instance.playerList[1].chooseNewCardAccess.DisplayCards();
            RefereeScript.instance.playerList[1].isThisPlayersTurnToChoose = false;
        }
        
    }


    [Command(requiresAuthority = false)]
    private void CmdSyncrhonizeCardDestruction()
    {
        RefereeScript.instance.playerList[0].chooseNewCardAccess.FindAndDestroyCard(9);
        //chooseNewCardAccess.FindAndDestroyCard(9);
    }

    [ClientRpc]
    public void DestroyCardAsServer()
    {
       
       Debug.Log("Card hopefully destroyed");
       RefereeScript.instance.playerList[1].chooseNewCardAccess.FindAndDestroyCard(9);
    }
    public void EndTurnPlayerScript()
    {
        //handScriptAccess.DisableAllCardsEvent();

        if (!isServer)
        {
            if (fieldScriptAccess.FieldHitCheck())
            {
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                CmdDealDamage(damageThisRound);

            }

            CmdEndTurn();
        }
        else if (isServer)
        {

            if (fieldScriptAccess.FieldHitCheck())
            {
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                RpcDealDamage(damageThisRound);
            }
            RpcEndTurn();
        }
        handScriptAccess.UtlCardsPlayedForOtherPlayer = 0;
    }

    [Command(requiresAuthority = false)]
    public void CmdDealDamage(int inputDamage)
    {
        if (isThisPlayersTurn)
        {
            RefereeScript.instance.playerList[0].DealDamageAsServer(inputDamage);
        }
    }
    [ClientRpc]
    public void RpcDealDamage(int inputDamage)
    {
        if (isThisPlayersTurn)
        {
            Debug.Log($"We have dealt {RefereeScript.instance.playerList[0].damageThisRound} amount of damage");
            //RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(RefereeScript.instance.playerList[0].damageThisRound);
            RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(inputDamage);
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        damageThisRound = 0;
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
    }



    [ClientRpc]
    public void DealDamageAsServer(int inputDamage2)
    {
        Debug.Log($"We have dealt {RefereeScript.instance.playerList[1].damageThisRound} amount of damage");
        //RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(RefereeScript.instance.playerList[1].damageThisRound);
        RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(inputDamage2);

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
        fieldScriptAccess.SpawnActiveCard(cardID);
    }

    [ClientRpc]
    private void RpcPlayCardForOtherPlayer(int cardID)
    {
        if (isClientOnly)
        {
            fieldScriptAccess.SpawnActiveCard(cardID);
        }
    }
}
