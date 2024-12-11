using System;
using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class TurnManagerMultiplayer : NetworkBehaviour
{
    [SyncVar] public bool IsPlayerATurn = true;
    [SyncVar] public bool IsPlayerBTurn = false;

    public List<GameObject> playerList;

    public static TurnManagerMultiplayer Instance;

    private void Awake()
    {
        if (Instance == null) {Instance = this;}
    }
    
    public void EndTurnMultiplayer()
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
        foreach (GameObject playerObject in playerList)
        {
            playerObject.GetComponent<PlayerScript>().TogglePlayerButtons();
        }
        foreach (GameObject playerObject in playerList)
        {
            playerObject.GetComponent<PlayerScript>().isThisPlayersTurn = !playerObject.GetComponent<PlayerScript>().isThisPlayersTurn;
        }
       
    }

}
