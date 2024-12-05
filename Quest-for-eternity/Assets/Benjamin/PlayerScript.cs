using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;

public class PlayerScript : NetworkBehaviour
{
    public GameObject EndTurnButton;
    TurnManagerMultiplayer turnManagerAccess;
    [SyncVar]
    public bool isHost;
    [SyncVar] 
    public bool isThisPlayersTurn;
    public void Start()
    {

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
        Debug.Log("ACTIVATED");
        if (isServer)
        {
            if (TurnManagerMultiplayer.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }
        }
        else if (!isServer)
        {
            if (TurnManagerMultiplayer.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
            }
        }

    }
    public void EndTurnPlayerScript()
    {
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
