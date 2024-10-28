using UnityEngine;

public class TurnScript : MonoBehaviour
{
    public FieldScript fieldScriptAccess;
    public HandScript handScriptAccess;

    public bool isPlayersTurn;

    [SerializeField]
    RefereeScript refereeScriptAccess;

    private void Start()
    {
        isPlayersTurn = true;
    }

    public void StartPlayersTurn()
    {
        isPlayersTurn = true;
    }

    private void EndPlayersTurn()
    {
        fieldScriptAccess.FieldClear();
        handScriptAccess.AddCardsToHand(0);
        isPlayersTurn = false;
      //  isPlayersTurn = refereeScriptAccess.EnemyAttack();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isPlayersTurn)
        {
            EndPlayersTurn();
        }
    }
}
