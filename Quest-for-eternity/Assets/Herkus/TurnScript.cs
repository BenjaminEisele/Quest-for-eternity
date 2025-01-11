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
    [HideInInspector]
    public bool isPlayersTurn;

    //[SerializeField]
    //RefereeScript refereeScriptAccess;

    [SerializeField]
    DeckManager deckManagerAccess;

    public delegate void EndTurnAction();
    public static event EndTurnAction endTurnEvent;

    public delegate void RestartGameAction();
    public static event RestartGameAction restartGameEvent;

    public static TurnScript instance;

    private void Awake()
    {
        if(instance == null) {  instance = this; }
    }

    //bool isSinglePlayer;

    private void Start()
    {
        PlayerScript myPlayerScript = GetComponentInChildren<PlayerScript>();
        //ShouldStartPlayerTurn(true);
        uiScriptAccess.ChangeEndTurnButtonStatus(true);
        isPlayersTurn = true;
        endTurnEvent += TransferTurnToEnemy;
        
    }   

    public bool GetPlayerTurnBool()
    {
        return isPlayersTurn;
    }

    public void SetPlayerTurnBool(bool inputBool)
    {
        isPlayersTurn = inputBool;
    }

    public void ShouldStartPlayerTurn(bool playerTurnBool)
    {
        if(!RefereeScript.instance.GetIsGameOver())
        {            
            if(playerTurnBool)
            {
                UiScript.UpdateTurnInfo(0);
                playerScriptAccess.EndTurnPlayerScript();
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
        Debug.Log(playerScriptAccess.gameObject.transform.root.name);
        if(isPlayersTurn)
        {
            
            UiScript.UpdateTurnInfo(1);
            isPlayersTurn = false;
            RefereeScript.instance.StartForeachEnemyCoroutine();
        }
    }

    public void RestartGame()
    {
        fieldScriptAccess.FieldClearAndDealDamage(false);
        deckManagerAccess.ResetAllCardLists();
        handScriptAccess.HandReset();
        RefereeScript.instance.RefereeReset();
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
            if (isPlayersTurn)
            {
                CallEndTurnEvent();
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
            //RestartGame();
            CallRestartGameEvent();
        }
    }
    
    private void CallRestartGameEvent()
    {
        restartGameEvent();
        isPlayersTurn = true;
    }
    public void CallEndTurnEvent()
    {
        Debug.Log("end turn event called");
        if (playerScriptAccess.isThisPlayersTurn)
        {
            if (endTurnEvent != null)
            {
                endTurnEvent();
            }
        }
        
        //uiScriptAccess.ChangeEndTurnButtonStatus(false);
    }
    
    /*[Command]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurn();
    }*/
}
