using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public GameObject ServerItemSpawnPoint;
    public GameObject ClientItemSpawnPoint;
    public GameObject ServerReadySpawnPoint;
    public GameObject ClientReadySpawnPoint;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;

    public Button StartGameButton;
    public TextMeshProUGUI ReadyButtonText;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ReadyPlayer()
    {
        LocalPlayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalPlayerController.Ready)
        {
            ReadyButtonText.text = "Not ready";
        }

        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (player.Ready)
            {
                AllReady = true;
            }

            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if (LocalPlayerController.PlayerIDNumber == 1)
            {
                StartGameButton.interactable = true;
            }

            else
            {
                StartGameButton.interactable = false;
            }
        }

        else
        {
            StartGameButton.interactable = false;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) { CreateHostPlayerItem(); }
        if (PlayerListItems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem(); }
        if (PlayerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); }
        if (PlayerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        Debug.Log("Create Host Item");
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConecctionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.server = true;
            NewPlayerItemScript.SetPlayerValues();
            NewPlayerItem.transform.SetParent(ServerItemSpawnPoint.transform);
            NewPlayerItem.transform.localPosition = Vector3.zero;
            NewPlayerItem.transform.GetChild(3).transform.SetParent(ServerReadySpawnPoint.transform);
            ServerReadySpawnPoint.transform.GetChild(0).localPosition = Vector3.zero;
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        PlayerObjectController host = Manager.GamePlayers[0];
        if (!PlayerListItems.Any(b => b.ConecctionID == host.ConnectionID))
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
            NewPlayerItemScript.PlayerName = host.PlayerName;
            NewPlayerItemScript.ConecctionID = host.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = host.PlayerSteamID;
            NewPlayerItemScript.Ready = host.Ready;
            NewPlayerItemScript.server = true;
            NewPlayerItemScript.SetPlayerValues();
            NewPlayerItem.transform.SetParent(ServerItemSpawnPoint.transform);
            NewPlayerItem.transform.localPosition = Vector3.zero;
            NewPlayerItem.transform.GetChild(3).transform.SetParent(ServerReadySpawnPoint.transform);
            ServerReadySpawnPoint.transform.GetChild(0).localPosition = Vector3.zero;
            NewPlayerItem.transform.localScale = Vector3.one;
            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerObjectController client = Manager.GamePlayers[1];
        if (!PlayerListItems.Any(b => b.ConecctionID == client.ConnectionID))
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
            NewPlayerItemScript.PlayerName = client.PlayerName;
            NewPlayerItemScript.ConecctionID = client.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = client.PlayerSteamID;
            NewPlayerItemScript.Ready = client.Ready;
            NewPlayerItemScript.server = false;
            NewPlayerItemScript.SetPlayerValues();
            NewPlayerItem.transform.SetParent(ClientItemSpawnPoint.transform);
            NewPlayerItem.transform.localPosition = Vector3.zero;
            NewPlayerItem.transform.GetChild(3).transform.SetParent(ClientReadySpawnPoint.transform);
            ClientReadySpawnPoint.transform.GetChild(0).localPosition = Vector3.zero;
            NewPlayerItem.transform.localScale = Vector3.one;
            PlayerListItems.Add(NewPlayerItemScript);
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConecctionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.SetPlayerValues();
                    if (player == LocalPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }

        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerlistItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerlistItem.ConecctionID))
            {
                playerListItemToRemove.Add(playerlistItem);
            }
        }

        if (playerListItemToRemove.Count > 0)
        {
            foreach(PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                PlayerListItems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }

    public void StartGame(string SceneName)
    {
        LocalPlayerController.CanStartGame(SceneName);
    }

    public void Quit()
    {
        LocalPlayerController.QuitCheck();
    }
}
