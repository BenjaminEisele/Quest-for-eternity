using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    private HandScript handScriptAccess;
    public Button EndTurnButton;
    public EnemyScript enemyScriptAccess;

    [SyncVar]
    public bool isHost;
    [SyncVar] 
    public bool isThisPlayersTurn;

    public int damageThisRound = 0;

    public void Start()
    {       
        TurnScript.endTurnEvent += EndTurnPlayerScript;

        if (isOwned)
        {
            if (isServer)
            {
                Debug.Log("I am the server");
                isThisPlayersTurn = true;
                isHost = true;
                EndTurnButton.interactable = true;
            }
            else //(!isServer)
            {
                Debug.Log("I am NOT the server");
                isThisPlayersTurn = false;
                isHost = false;
                EndTurnButton.interactable = false;
            }
        }
    }

    public void EndTurnPlayerScript()
    {      
        handScriptAccess.DisableAllCardsEvent();

        if (isClientOnly)
        {
            CmdEndTurn();
            RefereeScript.instance.targetEnemy.enemyHealth -= damageThisRound;
            Debug.Log("Attack1");
        }

        else if (isServer)
        {
            RpcEndTurn();
            Debug.Log("Attack2");
        }

        damageThisRound = 0;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
        RefereeScript.instance.targetEnemy.enemyHealth -= damageThisRound;
        Debug.Log("Attack3");
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
        Debug.Log("Attack4");
        if (isClientOnly)
        {
            RefereeScript.instance.targetEnemy.enemyHealth -= damageThisRound;
            Debug.Log("Attack5");
        }       
    }

}
