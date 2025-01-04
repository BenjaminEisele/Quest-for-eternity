using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    private HandScript handScriptAccess;
    public Button EndTurnButton;
    TurnManagerMultiplayer turnManagerAccess;
    [SyncVar]
    public bool isHost;
    [SyncVar] 
    public bool isThisPlayersTurn;

    public void Start()
    {
        TurnScript.endTurnEvent += EndTurnPlayerScript;
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            turnManagerAccess = TurnManagerMultiplayer.Instance;
            turnManagerAccess.playerList.Add(this.gameObject);
        }
        if (isOwned)
        {
            if (isServer)
            {
                Debug.Log("I am the server");
                isThisPlayersTurn = true;
                isHost = true;
                this.EndTurnButton.interactable = true;
            }
            else //(!isServer)
            {
                Debug.Log("I am NOT the server");
                isThisPlayersTurn = false;
                isHost = false;
                this.EndTurnButton.interactable = false;
            }
        }
    }

    public void TogglePlayerButtons()
    {
        Debug.Log("1");
        if (isHost)
        {
            Debug.Log("5");
            if (TurnManagerMultiplayer.Instance.IsPlayerATurn)
            {
                this.EndTurnButton.interactable = true;
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {                
                this.EndTurnButton.interactable = false;
            }
        }
        else if (!isHost)
        {
            Debug.Log("2");
            if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("3");               
                this.EndTurnButton.interactable = true;
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("4");          
                this.EndTurnButton.interactable = false;
            }
        }

    }
    public void EndTurnPlayerScript()
    {
        Debug.Log("End Turn called");
        if (!isServer)
        {
            CmdEndTurn();
            //isThisPlayersTurn = !isThisPlayersTurn;
            //this.EndTurnButton.interactable = isThisPlayersTurn;
        }

        else if (isServer)
        {
            RpcEndTurn();
        }

    }
    
    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
        //RpcEndTurn();
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
    }

}
