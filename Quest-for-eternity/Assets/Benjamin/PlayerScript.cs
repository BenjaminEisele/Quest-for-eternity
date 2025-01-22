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
    [SerializeField]
    FieldScript fieldScriptAccess;
    public bool shouldDealDamage;

    [SyncVar]
    public bool isHost;
    [SyncVar]
    public bool isThisPlayersTurn;

    //[SyncVar] public int damageThisRound = 0;

    [SyncVar] public int damageThisRound;


    public void Start()
    {
        RefereeScript.instance.playerList.Add(this);
        TurnScript.endTurnEvent += EndTurnPlayerScript;

        if (isOwned)
        {
            if (isServer)
            {
                // Debug.Log("I am the server");
                isThisPlayersTurn = true;
                isHost = true;
                EndTurnButton.interactable = true;
            }
            else //(!isServer)
            {
                // Debug.Log("I am NOT the server");
                isThisPlayersTurn = false;
                isHost = false;
                EndTurnButton.interactable = false;
            }
        }
    }



    public void EndTurnPlayerScript()
    {
        //handScriptAccess.DisableAllCardsEvent();

        if (!isServer)
        {
            if (fieldScriptAccess.FieldHitCheck())
            {
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                CmdDealDamage(damageThisRound);

            }

            CmdEndTurn();
        }
        else if (isServer)
        {

            if (fieldScriptAccess.FieldHitCheck())
            {
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                //Invoke("RpcDealDamage", 0.05f);
                RpcDealDamage(damageThisRound);
            }
            //Invoke("RpcEndTurn", 0.07f);
            RpcEndTurn();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDealDamage(int inputDamage)
    {
        if (isThisPlayersTurn)
        {
            RefereeScript.instance.playerList[0].DealDamageAsServer(inputDamage);
        }
    }
    [ClientRpc]
    public void RpcDealDamage(int inputDamage)
    {
        if (isThisPlayersTurn)
        {
            Debug.Log($"We have dealt {RefereeScript.instance.playerList[0].damageThisRound} amount of damage");
            //RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(RefereeScript.instance.playerList[0].damageThisRound);
            RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(inputDamage);
        }

    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        damageThisRound = 0;
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
    }



    [ClientRpc]
    public void DealDamageAsServer(int inputDamage2)
    {
        Debug.Log($"We have dealt {RefereeScript.instance.playerList[1].damageThisRound} amount of damage");
        //RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(RefereeScript.instance.playerList[1].damageThisRound);
        RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(inputDamage2);

    }
}
