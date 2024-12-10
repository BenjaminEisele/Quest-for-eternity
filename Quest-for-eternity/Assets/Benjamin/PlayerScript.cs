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
                // if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
                EndTurnButton.SetActive(!EndTurnButton.activeSelf);
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                // if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
                EndTurnButton.SetActive(!EndTurnButton.activeSelf);
            }
        }
        else if (!isHost)
        {
            Debug.Log("2");
            if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("3");
                //if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
                EndTurnButton.SetActive(!EndTurnButton.activeSelf);
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                Debug.Log("4");
                //if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
                EndTurnButton.SetActive(!EndTurnButton.activeSelf);
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
            TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        }

    }
    
    [Command]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
    }

    public void TestDebug()
    {
        Debug.Log("HAjfhöjfahljd");
    }

}
