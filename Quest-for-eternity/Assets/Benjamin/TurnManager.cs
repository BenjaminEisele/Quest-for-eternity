using System;
using Mirror;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    [SyncVar] public bool IsPlayerATurn = true;
    [SyncVar] public bool IsPlayerBTurn = false;

    [SerializeField] private GameObject EndTurnButton;
    
    public void EndTurn()
    {
        if (IsPlayerATurn)
        {
            IsPlayerATurn = false;
            IsPlayerBTurn = true;
            //EndTurnButton.SetActive(false);
            Debug.Log("1");
        }
        
        else if (IsPlayerBTurn)
        {
            IsPlayerATurn = true;
            IsPlayerBTurn = false;
            //EndTurnButton.SetActive(false);
            Debug.Log("2");
        }
    }
}
