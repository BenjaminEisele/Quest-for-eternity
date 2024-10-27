using UnityEngine;
using Steamworks;
using System.Collections.Generic;
public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;
    
    public GameObject LobbiesMenu;
    public GameObject LobbyDataItemPrefab;
    public GameObject LobbyListContent;

    public GameObject LobbiesButton, HostButton;

    public List<GameObject> LobbiesList = new List<GameObject>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void GetLobbiesList()
    {
        LobbiesButton.SetActive(false);
        HostButton.SetActive(false);
        LobbiesMenu.SetActive(true);
        SteamLobby.instance.GetLobbiesList();
    }

    public void Back()
    {
        LobbiesButton.SetActive(true);
        HostButton.SetActive(true);
        LobbiesMenu.SetActive(false);
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i=0; i<lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(LobbyDataItemPrefab);
                createdItem.GetComponent<LobbyEntryData>().LobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                createdItem.GetComponent<LobbyEntryData>().LobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
                createdItem.GetComponent<LobbyEntryData>().SetLobbyData();
                createdItem.transform.SetParent(LobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;
                LobbiesList.Add(createdItem);
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach (GameObject lobbyItem in LobbiesList)
        {
            Destroy(lobbyItem);
        }

        LobbiesList.Clear();
    }

}
