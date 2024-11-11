using UnityEngine;
using Mirror;
using UnityEditor.SearchService;

public class PlayerScript : NetworkBehaviour
{
    public GameObject EndTurnButton;

    public void EndTurn()
    {
        if (isClient)
        {
            CmdEndTurn();
        }

        else if (isServer)
        {
            TurnManagerScript.Instance.EndTurn();
        }
    }
    
    [Command]
    public void CmdEndTurn()
    {
        TurnManagerScript.Instance.EndTurn();
        //EndTurnButton.SetActive(false);
    }

    /*[ClientRpc]
    public void RpcUpdateEndTurnButton()
    {
        EndTurnButton.SetActive(false);
    }*/
}
