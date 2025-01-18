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
        handScriptAccess.DisableAllCardsEvent();

        if (!isServer)
        {
           
            if (fieldScriptAccess.FieldHitCheck())
            {
                CmdDealDamage();
            }
            CmdEndTurn();
        }
        else if (isServer)
        {
           
            if (fieldScriptAccess.FieldHitCheck())
            {
                damageThisRound = fieldScriptAccess.damagePointsLiquid;
                RpcDealDamage();
            }
            RpcEndTurn();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDealDamage()
    {
        if (isThisPlayersTurn)
        {
            damageThisRound = fieldScriptAccess.damagePointsLiquid;
            Test.instance.SubtractHealth();
            RefereeScript.instance.playerList[0].DealDamageAsServer();
        }
    }
    [ClientRpc]
    public void RpcDealDamage()
    {
        if (isThisPlayersTurn)
        {
            Test.instance.SubtractHealth();
            Debug.Log($"We have dealt {RefereeScript.instance.playerList[0].damageThisRound} amount of damage");
            RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(RefereeScript.instance.playerList[0].damageThisRound);
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
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
    }

   

    [ClientRpc]
    public void DealDamageAsServer()
    {
        Debug.Log($"We have dealt {RefereeScript.instance.playerList[1].damageThisRound} amount of damage");
        RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(RefereeScript.instance.playerList[1].damageThisRound);      
    }
}
