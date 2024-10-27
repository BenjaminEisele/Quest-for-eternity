using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class LobbyEntryData : MonoBehaviour
{
    //Data
    public CSteamID LobbyID;
    public string LobbyName;
    public Text LobbyNameText;

    public void SetLobbyData()
    {
        if (LobbyName == "")
        {
            LobbyNameText.text = "Empty";
        }

        else
        {
            LobbyNameText.text = LobbyName;
        }
    }

    public void JoinLobby()
    {
        SteamLobby.instance.JoinLobby(LobbyID);
    }

}
