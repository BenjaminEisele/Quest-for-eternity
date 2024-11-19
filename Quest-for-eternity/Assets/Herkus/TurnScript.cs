using UnityEngine;
using Mirror;

public class TurnScript : NetworkBehaviour
{
    public FieldScript fieldScriptAccess;
    public HandScript handScriptAccess;

    [SerializeField]
    private bool isPlayersTurn;

    [SerializeField]
    RefereeScript refereeScriptAccess;

    private void Start()
    {
        if (isServer)
        {
            isPlayersTurn = true;
        }

        if (!isServer)
        {
            isPlayersTurn = false;
        }
    }

    public bool GetPlayerTurnBool()
    {
        return isPlayersTurn;
    }

    public void SetPlayerTurnBool(bool inputBool)
    {
        isPlayersTurn = inputBool;
    }
   
   /* public void StartPlayersTurn()
    {
        isPlayersTurn = true;
    }*/

    public void ShouldStartPlayerTurn()
    {
        if(!refereeScriptAccess.GetIsGameOver())
        {
            isPlayersTurn = !isPlayersTurn;
           
        }   
    }

    public void EndPlayersTurn()
    {
        if(isPlayersTurn)
        {
            UiScript.UpdateTurnInfo(1);
            fieldScriptAccess.FieldClearAndDealDamage(true);
            handScriptAccess.AddCardsToHand(0);
            isPlayersTurn = false;
            refereeScriptAccess.EnemyAttack();
        }
    }

    private void RestartGame()
    {
        fieldScriptAccess.FieldClearAndDealDamage(false);
        handScriptAccess.HandReset();
        refereeScriptAccess.RefereeReset();
        isPlayersTurn = true;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EndPlayersTurn();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
}
