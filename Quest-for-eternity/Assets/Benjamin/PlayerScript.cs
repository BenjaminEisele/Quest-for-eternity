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

    public void EndTurnPlayerScript()
    {      
        handScriptAccess.DisableAllCardsEvent();

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
