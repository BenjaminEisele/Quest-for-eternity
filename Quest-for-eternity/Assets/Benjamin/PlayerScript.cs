using UnityEngine;
using Mirror;
using UnityEditor.SearchService;

public class PlayerScript : NetworkBehaviour
{
    public GameObject EndTurnButton;

    public void Update()
    {
        if (isOwned)
        {
            if (TurnManagerScript.Instance.IsPlayerATurn == true) {EndTurnButton.SetActive(true);}
        }

        if (!isOwned) //evtl !isServer
        {
            if (TurnManagerScript.Instance.IsPlayerBTurn == true) {EndTurnButton.SetActive(true);}    
        }
    }

    public void EndTurn()
    {
        if (isOwned)
        {
            CmdEndTurn();
            EndTurnButton.SetActive(false);
        }

        else if (!isOwned)
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
