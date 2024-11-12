using UnityEngine;
using Mirror;
using UnityEditor.SearchService;

public class PlayerScript : NetworkBehaviour
{
    public GameObject EndTurnButton;

    public void Update()
    {
        if (isServer)
        {
            if (TurnManagerScript.Instance.IsPlayerATurn == true) {EndTurnButton.SetActive(true);}
        }

        if (isClient) //evtl !isServer
        {
            if (TurnManagerScript.Instance.IsPlayerBTurn == true) {EndTurnButton.SetActive(true);}    
        }
    }

    public void EndTurn()
    {
        if (isClient)
        {
            CmdEndTurn();
            EndTurnButton.SetActive(false);
        }

        else if (isServer)
        {
            TurnManagerScript.Instance.EndTurn();
            EndTurnButton.SetActive(false);
        }
    }
    
    [Command]
    public void CmdEndTurn()
    {
        TurnManagerScript.Instance.EndTurn();
    }

}
