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
                if (this.EndTurnButton.activeSelf == false)
                {
                    UpdateTurnButton(true);
                }
            }
        }
        
        else if (PlayerObjectController.Instance.ConnectionID >= 0)
        {
            if (IsPlayerBTurn)
            {
                if (this.EndTurnButton.activeSelf == false)
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
        }
        
        else if (PlayerObjectController.Instance.ConnectionID >= 0)
        {
            IsPlayerATurn = true;
            IsPlayerBTurn = false;
        }
    }
    
    public void UpdateTurnButton(bool input)
    {
        if (PlayerObjectController.Instance.ConnectionID == 0)
        {
            this.EndTurnButton.SetActive(input);
        }
        
        else if (PlayerObjectController.Instance.ConnectionID >= 0)
        {
            this.EndTurnButton.SetActive(!input);
        }
    }

    [Command]
    public void CmdUpdateTurnButton(bool input)
    {
        if (PlayerObjectController.Instance.ConnectionID == 0)
        {
            this.EndTurnButton.SetActive(input);
        }
        
        else if (PlayerObjectController.Instance.ConnectionID >= 0)
        {
            this.EndTurnButton.SetActive(!input);
        }
    }
}
