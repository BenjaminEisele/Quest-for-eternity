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
        
        
        endTurnEvent += EndPlayersTurn;
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

    public void EndPlayersTurn()
    {
        //turnManagerMultiplayer.
        if(isPlayersTurn)
        {
            UiScript.UpdateTurnInfo(1);
            Debug.Log("added cards from turn script");
            handScriptAccess.AddCardsToHand(0);
            //handScriptAccess.AddCardsToHand(0);
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
                    if (endTurnEvent != null)
                    {
                        endTurnEvent();
                    }
                    uiScriptAccess.ChangeEndTurnButtonStatus(false);
                }
            }
            else
            {
                if (playerScriptAccess.isThisPlayersTurn && playerScriptAccess.isOwned)
                {
                    if (endTurnEvent != null)
                    {
                        endTurnEvent();
                    }
                    uiScriptAccess.ChangeEndTurnButtonStatus(false);
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

    /*[Command]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurn();
    }*/
}
