using UnityEngine;
using Mirror;

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
            if (TurnManagerMultiplayer.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
            }

            if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }
        }

        if (!isServer)
        {
            if (TurnManagerMultiplayer.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }

            if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
            }
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
            TurnManagerMultiplayer.Instance.EndTurn();
        }

    }
    
    [Command]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurn();
    }

}
