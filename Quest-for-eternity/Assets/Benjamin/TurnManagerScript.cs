using System;
using Mirror;
using UnityEngine;

public class TurnManagerScript : NetworkBehaviour
{
    [SyncVar] public bool IsPlayerATurn = true;
    [SyncVar] public bool IsPlayerBTurn = false;

    public static TurnManagerScript Instance;

    private void Awake()
    {
        if (Instance == null) {Instance = this;}
    }
   
    public void EndTurn()
    {
        if (IsPlayerATurn)
        {
            IsPlayerATurn = false;
            IsPlayerBTurn = true;
        }
        
        else if (IsPlayerBTurn)
        {
            IsPlayerATurn = true;
            IsPlayerBTurn = false;
        }
    }

    
}
