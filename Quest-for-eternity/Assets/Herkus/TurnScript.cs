using UnityEngine;
using Mirror;

public class TurnScript : MonoBehaviour
{
    public FieldScript fieldScriptAccess;
    public HandScript handScriptAccess;
    public PlayerScript playerScriptAccess;
    public UiScript uiScriptAccess;
    [SerializeField]
    //[SyncVar]
    private bool isPlayersTurn;

    [SerializeField]
    RefereeScript refereeScriptAccess;

    [SerializeField]
    DeckManager deckManagerAccess;

    TurnManagerMultiplayer turnManagerAccess;

    public delegate void EndTurnAction();
    public static event EndTurnAction endTurnEvent;

    bool isSinglePlayer;
    private void Start()
    {
        PlayerScript myPlayerScript = GetComponentInChildren<PlayerScript>();
        if(myPlayerScript != null)
        {
            playerScriptAccess = myPlayerScript;
            isSinglePlayer = false;
        }
        else
        {
            isSinglePlayer = true;
        }
        
        
        endTurnEvent += TransferTurnToEnemy;
        turnManagerAccess = TurnManagerMultiplayer.Instance;
        //ShouldStartPlayerTurn(true);
        uiScriptAccess.ChangeEndTurnButtonStatus(true);
        isPlayersTurn = true;
    }


    

    public bool GetPlayerTurnBool()
    {
        return isPlayersTurn;
    }

    public void SetPlayerTurnBool(bool inputBool)
    {
        isPlayersTurn = inputBool;
    }
   
   /* public void StartPlayersTurn()
    {
        isPlayersTurn = true;
    }*/

    public void ShouldStartPlayerTurn(bool playerTurnBool)
    {
        if(!refereeScriptAccess.GetIsGameOver())
        {
            isPlayersTurn = playerTurnBool;
            if(playerTurnBool)
            {
                UiScript.UpdateTurnInfo(0);
            }
            else
            {
                UiScript.UpdateTurnInfo(1);
            }
        }
        uiScriptAccess.ChangeEndTurnButtonStatus(playerTurnBool);
    }

    public void TransferTurnToEnemy()
    {
        if(isPlayersTurn)
        {
            UiScript.UpdateTurnInfo(1);
            isPlayersTurn = false;
            refereeScriptAccess.StartEnemyCoroutines();
        }
    }

    public void RestartGame()
    {
        fieldScriptAccess.FieldClearAndDealDamage(false);
        deckManagerAccess.ResetAllCardLists();
        handScriptAccess.HandReset();
        refereeScriptAccess.RefereeReset();
        isPlayersTurn = true;
    }

    /*public void EndTurn()
    {
        if (!isServer)
        {
            CmdEndTurn();
        }

        else if (isServer)
        {
            TurnManagerMultiplayer.Instance.EndTurn();
        }

    }*/

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isSinglePlayer)
            {
                if (isPlayersTurn)
                {
                    CallEndTurnEvent();
                }
            }
            else
            {
                if (playerScriptAccess.isThisPlayersTurn && playerScriptAccess.isOwned)
                {
                    CallEndTurnEvent();
                    /*
                    EndPlayersTurn();
                    handScriptAccess.AddCardsToHand(0);
                    fieldScriptAccess.FieldClearAndDealDamage(true);
                    Debug.Log(transform.parent.parent.name);
                    playerScriptAccess.EndTurnPlayerScript();
                    */
                }
            }
          
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    public void CallEndTurnEvent()
    {
        if (endTurnEvent != null)
        {
            endTurnEvent();
        }
        uiScriptAccess.ChangeEndTurnButtonStatus(false);
    }
    /*[Command]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurn();
    }*/
}
