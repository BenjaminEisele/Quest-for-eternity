using UnityEngine;
using Mirror;

public class TurnScript : MonoBehaviour
{
    [SerializeField]
    HandScript handScriptAccess;
    PlayerScript playerScriptAccess;
    [SerializeField]
    UiScript uiScriptAccess;

    public bool isPlayersTurn;

    [SerializeField]
    DeckManager deckManagerAccess;

    [SerializeField] VoiceManager voiceManager;
    

    public delegate void EndTurnAction();
    public event EndTurnAction endTurnEvent;



    bool isSinglePlayer;

    public static TurnScript instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PlayerScript myPlayerScript = transform.root.GetComponentInChildren<PlayerScript>();
        if(myPlayerScript != null)
        {
            playerScriptAccess = myPlayerScript;
            isSinglePlayer = false;
        }
        else
        {
            isSinglePlayer = true;
        }
        endTurnEvent += TransferTurnToEnemy;
        uiScriptAccess.ChangeEndTurnButtonStatus(true);
        isPlayersTurn = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSinglePlayer)
            {
                if (isPlayersTurn)
                {
                    CallEndTurnEvent();
                }
            }
            else
            {
                if (playerScriptAccess.isThisPlayersTurn && playerScriptAccess.isOwned)
                {
                    CallEndTurnEvent();
                }
            }
        }
    }
    public void CallEndTurnEvent()
    {
        if (playerScriptAccess.isThisPlayersTurn)
        {
            if (endTurnEvent != null)
            {
                endTurnEvent();
                if (Random.Range(0, 101) < 10)
                {
                    voiceManager.PlayersTurnLine();
                }
            }
            //uiScriptAccess.ChangeEndTurnButtonStatus(false);
        }
    }

    public void SetPlayerTurnBool(bool inputBool)
    {
        isPlayersTurn = inputBool;
    }

    public void ShouldStartPlayerTurn(bool playerTurnBool)
    {
        if(!RefereeScript.instance.GetIsGameOver())
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
        uiScriptAccess.ChangeEndTurnButtonStatus(playerTurnBool);
    }

    public void TransferTurnToEnemy()
    {
        if(isPlayersTurn)
        {
            UiScript.UpdateTurnInfo(1);
            isPlayersTurn = false;
            RefereeScript.instance.StartForeachEnemyCoroutine();
        }
    }

    public void RestartGame()
    {
        deckManagerAccess.ResetAllCardLists();
        handScriptAccess.HandReset();
        RefereeScript.instance.RefereeReset();
        isPlayersTurn = true;
    } 
}
