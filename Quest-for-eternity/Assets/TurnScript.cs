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
      
    }

    public bool GetPlayerTurnBool()
    {
        return isPlayersTurn;
    }
   
   /* public void StartPlayersTurn()
    {
        isPlayersTurn = true;
    }*/

    public void StartPlayerTurn(bool playerTurnBool)
    {
        if(!refereeScriptAccess.GetIsGameOver())
        {
            isPlayersTurn = playerTurnBool;
        }
       
    }

    private void EndPlayersTurn()
    {
        fieldScriptAccess.FieldClearAndDealDamage(true);
        handScriptAccess.AddCardsToHand(0);
        isPlayersTurn = false;
        //  isPlayersTurn = 
        refereeScriptAccess.EnemyAttack();
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
        if(Input.GetKeyDown(KeyCode.Space) && isPlayersTurn)
        {
            EndPlayersTurn();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
}
