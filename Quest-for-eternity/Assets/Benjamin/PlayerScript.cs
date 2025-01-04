using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
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
                EndTurnButton.interactable = true;
            }
            else //(!isServer)
            {
                Debug.Log("I am NOT the server");
                isThisPlayersTurn = false;
                isHost = false;
                EndTurnButton.interactable = false;
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
                EndTurnButton.interactable = true;
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {                
                EndTurnButton.interactable = false;
            }
        }
        else if (!isHost)
        {
            Debug.Log("2");
            if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("3");               
                EndTurnButton.interactable = true;
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("4");          
                EndTurnButton.interactable = false;
            }
        }

    }
    public void EndTurnPlayerScript()
    {
        Debug.Log("End Turn called");
        if (!isServer)
        {
            CmdEndTurn();
            isThisPlayersTurn = !isThisPlayersTurn;
            EndTurnButton.interactable = isThisPlayersTurn;
        }

        else if (isServer)
        {
            RpcEndTurn();
        }

    }
    
    [Command]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        isThisPlayersTurn = !isThisPlayersTurn;
        EndTurnButton.interactable = isThisPlayersTurn;
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        isThisPlayersTurn = !isThisPlayersTurn;
        EndTurnButton.interactable = isThisPlayersTurn;
    }

}
