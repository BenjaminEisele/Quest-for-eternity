using UnityEngine;
using UnityEngine.SceneManagement;

using Mirror;

public class PlayerScript : NetworkBehaviour
{
    public GameObject EndTurnButton;
    TurnManagerMultiplayer turnManagerAccess;

    public void Start()
    {

        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            turnManagerAccess = TurnManagerMultiplayer.Instance;
            turnManagerAccess.playerList.Add(this.gameObject);
        }
        if (isServer)
        {
            EndTurnButton.SetActive(true);
        }

        if (!isServer)
        {
            EndTurnButton.SetActive(false);
        }


    }

    public void TogglePlayerButtons()
    {
        if (isServer)
        {
            if (TurnManagerMultiplayer.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }
        }
        else if (!isServer)
        {
            if (TurnManagerMultiplayer.Instance.IsPlayerATurn)
            {
                if (EndTurnButton.activeSelf == true) { EndTurnButton.SetActive(false); }
            }
            else //if (TurnManagerMultiplayer.Instance.IsPlayerBTurn)
            {
                if (EndTurnButton.activeSelf == false) { EndTurnButton.SetActive(true); }
            }
        }

    }
    public void EndTurnPlayerScript()
    {
        if (!isServer)
        {
            CmdEndTurn();   
        }

        else if (isServer)
        {
            TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
        }

    }
    
    [Command]
    public void CmdEndTurn()
    {
        TurnManagerMultiplayer.Instance.EndTurnMultiplayer();
    }

    public void TestDebug()
    {
        Debug.Log("HAjfhöjfahljd");
    }

}
