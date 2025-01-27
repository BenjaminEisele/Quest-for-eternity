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
        //isThisPlayersTurnToChoose = true;



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
        TurnScript.endTurnEvent += EndTurnPlayerScript;
    }


    private void Update()
    {
     /*   if (Input.GetKeyDown(KeyCode.O))
        {
            if(isHost)
            {
                /*isThisPlayersTurnToChoose = true;
                RefereeScript.instance.CallPreNewWaveEvent();
                CallNewCardsAsServer();
                BeginPreNewWaveCall();
            }
        } */
    }

    public void BeginPreNewWaveCall()
    {
        Debug.Log(transform.root.gameObject.name);
        if(isThisPlayersTurn)
        {
            if (isHost && isServer)
            {
                Debug.Log("server sets the bools");
                RefereeScript.instance.playerList[0].isThisPlayersTurnToChoose = true;
                RefereeScript.instance.playerList[1].isThisPlayersTurnToChoose = false;
                RefereeScript.instance.CallPreNewWaveEvent();
                CallNewCardsAsServer();
            }
            else if (isClientOnly)
            {
                Debug.Log("client sets the bools");
                CmdPreNewWaveCall();
            }
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdPreNewWaveCall()
    {
        if(!isHost)
        {
            RefereeScript.instance.playerList[0].isThisPlayersTurnToChoose = false;
            RefereeScript.instance.playerList[1].isThisPlayersTurnToChoose = true;

            RefereeScript.instance.CallPreNewWaveEvent();
            CallNewCardsAsServer();
        }
       
    }

    public void BeginDisplayCardSynchronization(int inputId)
    {
        Debug.Log("synchronization called");
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

    public void DisplayCardsCallNest()
    {
       DisplayCardsCall();
    }
    [ClientRpc]
    public void DisplayCardsCall()
    {
        transform.root.GetComponentInChildren<ChooseNewCardScript>().DisplayCardsHidden();
    }

    [ClientRpc]
    public void CallNewCardsAsServer()
    {
        //RefereeScript.instance.CallPreNewWaveEvent();
        //Debug.Log("Card hopefully added");
        if (isClientOnly)
        {
            RefereeScript.instance.playerList[1].chooseNewCardAccess.DisplayCards();
            //RefereeScript.instance.playerList[1].isThisPlayersTurnToChoose = false;
        } 
        
    }


    [Command(requiresAuthority = false)]
    private void CmdSyncrhonizeCardDestruction(int inputId)
    {
       // RefereeScript.instance.playerList[0].chooseNewCardAccess.FindAndDestroyCard(inputId);
        DestroyCardAsClient(inputId);
        //chooseNewCardAccess.FindAndDestroyCard(9);
    }

    [ClientRpc]
    public void DestroyCardAsServer(int inputId)
    {
       
       Debug.Log("Card hopefully destroyed");
       RefereeScript.instance.playerList[1].chooseNewCardAccess.FindAndDestroyCard(inputId);
    }
    [ClientRpc]
    public void DestroyCardAsClient(int inputId)
    {

        Debug.Log("Card hopefully destroyed");
        RefereeScript.instance.playerList[0].chooseNewCardAccess.FindAndDestroyCard(inputId);
    }
    public void EndTurnPlayerScript()
    {
            handScriptAccess.DisableAllCardsEvent();
            Debug.Log("my name is: " + transform.root.gameObject.name);
            if (!isServer)
            {
                if (fieldScriptAccess.FieldHitCheck())
                {
                    damageThisRound = fieldScriptAccess.damagePointsLiquid;
                    CmdDealDamage(damageThisRound);

                }
                Invoke("CmdEndTurn", 0.1f);
                //CmdEndTurn();                       
            }
            else if (isServer)
            {

                if (fieldScriptAccess.FieldHitCheck())
                {
                    damageThisRound = fieldScriptAccess.damagePointsLiquid;
                    RpcDealDamage(damageThisRound);
                }
                Invoke("RpcEndTurn", 0.1f);
                // RpcEndTurn();            
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
            //Debug.Log($"We have dealt {RefereeScript.instance.playerList[0].damageThisRound} amount of damage");
            //RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(RefereeScript.instance.playerList[0].damageThisRound);
            RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(inputDamage);
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



    [ClientRpc]
    public void DealDamageAsServer(int inputDamage2)
    {
        //Debug.Log($"We have dealt {RefereeScript.instance.playerList[1].damageThisRound} amount of damage");
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
