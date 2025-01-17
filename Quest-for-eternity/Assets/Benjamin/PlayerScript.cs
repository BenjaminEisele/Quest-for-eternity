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

    [SyncVar]
    public bool isHost;
    [SyncVar] 
    public bool isThisPlayersTurn;

    [SyncVar] public int damageThisRound = 0;

    public void Start()
    {
        RefereeScript.instance.playerList.Add(this);
        //Debug.Log(RefereeScript.instance.targetEnemy.gameObject.GetInstanceID());
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
        Debug.Log("HELLO");

        if (!isServer)
        {
            CmdEndTurn();
        }

        else if (isServer)
        {
            RpcEndTurn();
        }
        damageThisRound = 0;
        //   damageThisRound = 3;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn()
    {
         if (isThisPlayersTurn)
         {
             Test.instance.SubtractHealth();
            //RefereeScript.instance.playerList[0].CallForClient(RefereeScript.instance.targetEnemy, damageThisRound);
            RefereeScript.instance.playerList[0].DealDamageAsServer();
             Debug.Log($"Total damage is : {damageThisRound}");
            
         }
        isThisPlayersTurn = !isThisPlayersTurn;
         this.EndTurnButton.interactable = isThisPlayersTurn;
         handScriptAccess.ActivateAllCardsEvent();

         
        //RefereeScript.instance.playerList[0].RpcEndTurn();

    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        if (isThisPlayersTurn)
        {
            Test.instance.SubtractHealth();
            RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(damageThisRound);
            

            //RefereeScript.instance.dealDamageToEnemy(damageThisRound, RefereeScript.instance.targetEnemy);

            //RefereeScript.instance.playerList[1].GetComponent<FieldScript>().FieldClearAndDealDamage(true);
            Debug.Log($"Total damage is : {damageThisRound}");
        }
        isThisPlayersTurn = !isThisPlayersTurn;
        this.EndTurnButton.interactable = isThisPlayersTurn;
        handScriptAccess.ActivateAllCardsEvent();
    }

    [ClientRpc]
    public void CallForClient(EnemyScript enemyReference, float inputDamage)
    {
        Debug.Log("hahahahah");
        //Debug.Log(RefereeScript.instance.playerList[1].gameObject.name);
        //RefereeScript.instance.playerList[1].transform.parent.GetComponentInChildren<FieldScript>().FieldClearAndDealDamage(true);
        /*if (RefereeScript.instance.playerList[1].fieldScriptAccess.FieldClearAndCheckIfHit())
        {
            //RefereeScript.instance.playerList[1].
            enemyReference.TakeDamageAndCheckIfDead((int)inputDamage);
        }
        else
        {
            Debug.Log("hit failed inside of player script");
        }
        */
        //RefereeScript.instance.dealDamageToEnemy(damageThisRound);
        //RefereeScript.instance.targetEnemy.TakeDamageAndCheckIfDead(damageThisRound);
    }

    [ClientRpc]
    public void DealDamageAsServer()
    {
        Debug.Log("ABC");
    }
}
