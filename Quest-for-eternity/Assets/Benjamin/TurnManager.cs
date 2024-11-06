using System;
using Mirror;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    [SyncVar] public bool IsPlayerATurn = true;
    [SyncVar] public bool IsPlayerBTurn = false;

    [SerializeField] private GameObject EndTurnButton;
    
    public void Update()
    {
        if (PlayerObjectController.Instance.ConnectionID == 0)
        {
            if (IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == false)
                {
                    UpdateTurnButton(true);
                }
            }
        }
        
        else if (PlayerObjectController.Instance.ConnectionID > 0)
        {
            if (IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == false)
                {
                    CmdUpdateTurnButton(false);
                }
            }
        }
    }

    public void EndTurn()
    {
        if (PlayerObjectController.Instance.ConnectionID == 0)
        {
            IsPlayerATurn = false;
            IsPlayerBTurn = true;
            EndTurnButton.SetActive(false);
            Debug.Log("1");
        }
        
        else if (PlayerObjectController.Instance.ConnectionID > 0)
        {
            IsPlayerATurn = true;
            IsPlayerBTurn = false;
            EndTurnButton.SetActive(false);
            Debug.Log("2");
        }
    }
    
    public void UpdateTurnButton(bool input)
    {
        if (PlayerObjectController.Instance.ConnectionID == 0)
        {
            EndTurnButton.SetActive(input);
            Debug.Log("3");
        }
        
        else if (PlayerObjectController.Instance.ConnectionID > 0)
        {
            EndTurnButton.SetActive(!input);
            Debug.Log("4");
        }
    }

    [Command]
    public void CmdUpdateTurnButton(bool input)
    {
        if (PlayerObjectController.Instance.ConnectionID == 0)
        {
            EndTurnButton.SetActive(input);
            Debug.Log("5");
        }
        
        else if (PlayerObjectController.Instance.ConnectionID > 0)
        {
            EndTurnButton.SetActive(!input);
            Debug.Log("6");
        }
    }
    
}
