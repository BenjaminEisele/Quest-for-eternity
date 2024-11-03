using Mirror;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    [SyncVar] public bool IsPlayerATurn = true;
    [SyncVar] public bool IsPlayerBTurn = false;

    [SerializeField] private GameObject EndTurnButtonA;
    [SerializeField] private GameObject EndTurnButtonB;

    public void PlayerAEndTurn()
    { 
        this.IsPlayerATurn = false;
        this.IsPlayerBTurn = true;
        //UpdateTurn(false);
        CmdUpdateTurn(false);
    }

    public void PlayerBEndTurn()
    {
        this.IsPlayerATurn = true;
        this.IsPlayerBTurn = false;
        //UpdateTurn(true);
        CmdUpdateTurn(true);
    }

    /*public void UpdateTurn(bool input)
    {
        this.EndTurnButtonA.SetActive(input);
        this.EndTurnButtonB.SetActive(!input);
    }*/

    [Command]
    public void CmdUpdateTurn(bool input)
    {
        this.EndTurnButtonA.SetActive(input);
        this.EndTurnButtonB.SetActive(!input);
    }
}
