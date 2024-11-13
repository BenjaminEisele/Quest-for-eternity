using UnityEngine;
using Mirror;
using UnityEditor.SearchService;

public class PlayerScript : NetworkBehaviour
{
    public GameObject EndTurnButton;

    public void Start()
    {
        if (isServer)
        {
            EndTurnButton.SetActive(true);
        }

        if (!isServer)
        {
            EndTurnButton.SetActive(false);
        }
    }

    public void Update()
    {
        
        if (isServer)
        {
            if (TurnManagerScript.Instance.IsPlayerATurn) {EndTurnButton.SetActive(true);}
            if (TurnManagerScript.Instance.IsPlayerBTurn) {EndTurnButton.SetActive(false);}
        }

        if (!isServer)
        {
            if (TurnManagerScript.Instance.IsPlayerATurn) {EndTurnButton.SetActive(false);}
            if (TurnManagerScript.Instance.IsPlayerBTurn) {EndTurnButton.SetActive(true);}
        }
        
    }

    public void EndTurn()
    {
        if (!isServer)
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
    }

}
