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
        Debug.Log("Wtf");
        handScriptAccess.DisableAllCardsEvent();

        if (!isServer)
        {
            CmdEndTurn();
        }

        else if (isServer)
        {
            RpcEndTurn();
        }

        damageThisRound = 0;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        Debug.Log("WTF1");
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
        Test.instance.SubtractHealth();
        //RefereeScript.instance.targetEnemy.enemyHealth -= damageThisRound;
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        Debug.Log("WTF2");
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
        Test.instance.SubtractHealth();
        //RefereeScript.instance.targetEnemy.enemyHealth -= damageThisRound;       
    }

}
