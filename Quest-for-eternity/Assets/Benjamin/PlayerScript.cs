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

        if (!isServer)
        {
            if (TurnManagerScript.Instance.IsPlayerBTurn == true) {EndTurnButton.SetActive(true);}    
        }
    }

    public void EndTurn()
    {
        if (!isServer)
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
