using UnityEngine;

public class TurnScript : MonoBehaviour
{
    public FieldScript fieldScriptAccess;
    public HandScript handScriptAccess;

    [SerializeField]
    private bool isPlayersTurn;

    [SerializeField]
    RefereeScript refereeScriptAccess;

    private void Start()
    {
        isPlayersTurn = true;
        // UiScript.UpdateTurnInfo(0);
       // ShouldStartPlayerTurn(true);
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

    public void ShouldStartPlayerTurn(bool playerTurnBool)
    {
        if(!refereeScriptAccess.GetIsGameOver())
        {
            isPlayersTurn = playerTurnBool;
            if(playerTurnBool)
            {
                UiScript.UpdateTurnInfo(0);
            }
            else
            {
                UiScript.UpdateTurnInfo(1);
            }
        }   
    }

    public void EndPlayersTurn()
    {
        if(isPlayersTurn)
        {
            UiScript.UpdateTurnInfo(1);
           
            //handScriptAccess.AddCardsToHand(0);
            isPlayersTurn = false;
            refereeScriptAccess.StartEnemyCoroutines();
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
