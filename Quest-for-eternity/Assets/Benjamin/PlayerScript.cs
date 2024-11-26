using UnityEngine;
using Mirror;
//using UnityEditor.SearchService;
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
            if (TurnManagerScript.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
            }

            if (TurnManagerScript.Instance.IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }
        }

        if (!isServer)
        {
            if (TurnManagerScript.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }

            if (TurnManagerScript.Instance.IsPlayerBTurn)
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
            TurnManagerScript.Instance.EndTurn();
        }

    }
    
    [Command]
    public void CmdEndTurn()
    {
        TurnManagerScript.Instance.EndTurn();
    }

}
