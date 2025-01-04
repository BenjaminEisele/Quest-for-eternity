using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;

public class PlayerScript : NetworkBehaviour
{
    //[SyncVar]
    public GameObject EndTurnButton;
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
                EndTurnButton.SetActive(true);
            }
            else //(!isServer)
            {
                Debug.Log("I am NOT the server");
                isThisPlayersTurn = false;
                isHost = false;
                EndTurnButton.SetActive(false);
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
                EndTurnButton.SetActive(true);
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {                
                EndTurnButton.SetActive(false);
            }
        }
        else if (!isHost)
        {
            Debug.Log("2");
            if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("3");               
                EndTurnButton.SetActive(true);
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("4");          
                EndTurnButton.SetActive(false);
            }
        }

    }
    public void EndTurnPlayerScript()
    {
        Debug.Log("End Turn called");
        if (!isServer)
        {
            CmdEndTurn();   
        }

        else if (isServer)
        {
            RpcEndTurn();
        }

    }
    
    [Command]
    public void CmdEndTurn()
    {
        //TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        isThisPlayersTurn = !isThisPlayersTurn;
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        //TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        isThisPlayersTurn = !isThisPlayersTurn;
    }

}
