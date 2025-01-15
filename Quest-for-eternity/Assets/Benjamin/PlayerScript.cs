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

    private int damageThisRound = 3;

    public void Start()
    {
        Debug.Log(RefereeScript.instance.targetEnemy.gameObject.GetInstanceID());
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
        Debug.Log(gameObject.transform.root.name);
        if (!isServer)
        {
            CmdEndTurn();
        }

        else if (isServer)
        {
            RpcEndTurn();
        }

        damageThisRound = 3;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        if (isThisPlayersTurn)
        {
            Test.instance.SubtractHealth();
            //RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(damageThisRound);
            Debug.Log(RefereeScript.instance.targetEnemy.gameObject.GetInstanceID());
            //RefereeScript.instance.targetEnemy.enemyHealth -= damageThisRound;
        }
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
        Debug.Log("we did this amount of damage:" + damageThisRound);
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        if (isThisPlayersTurn)
        {
            Test.instance.SubtractHealth();
            RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(damageThisRound);
            Debug.Log(RefereeScript.instance.targetEnemy.gameObject.GetInstanceID());
            //RefereeScript.instance.targetEnemy.enemyHealth -= damageThisRound;
        }
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
        Debug.Log("we did this amount of damage:" +  damageThisRound);
    }

}
