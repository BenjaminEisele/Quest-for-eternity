using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;

public class PlayerScript : NetworkBehaviour
{
    //[SyncVar]
    public GameObject EndTurnButton;
    [SyncVar]
    public bool isHost;
    [SyncVar] 
    public bool isThisPlayersTurn;
    public void Start()
    {
        TurnScript.endTurnEvent += EndTurnPlayerScript;

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
