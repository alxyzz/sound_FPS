using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Steamworks;
using System.Collections.Generic;
using System;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TMP_Text lobbyNameText;

    //Player Data
    public GameObject playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    //Other Data
    public ulong currentLobbyID;
    public bool playerItemCreated;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public PlayerController LocalPlayerController;

    //Ready
    public Button startGameButton;
    public TMP_Text readyButtonText;

    //Manager
    private FPSNetworkManager manager;
    private FPSNetworkManager Manager {
        get
        {
            if (manager != null) {
                return manager;
            }

            return manager = FPSNetworkManager.singleton as FPSNetworkManager;
        }
    }

    void Awake() {
        if (Instance == null) Instance = this;

        }

    public void ReadyPlayer() {
        LocalPlayerController.ChangeReady();
    }

    public void UpdateButton() {
        if (LocalPlayerController.isReady) {
            readyButtonText.text = "Not Ready";
        } else {
            readyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady() {
        bool isAllReady = false;

        foreach (PlayerController player in Manager.gamePlayers)
        {
            if (player.isReady) isAllReady = true;
            else {
                isAllReady = false;
                break;
            }
        }

        if (isAllReady) {
            if (LocalPlayerController.playerIDNumber == 1) {
                startGameButton.interactable = true;
            } else {
                startGameButton.interactable = false;
            }
        } else {
            startGameButton.interactable = false;
        }
    }

    public void UpdateLobbyName() {
        currentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        //Debug.Log("got here @ LobbyController.UpdatePlayerList()");
        if (!playerItemCreated) CreateHostPlayerItem(); //Host
        if (playerListItems.Count < Manager.gamePlayers.Count) CreateClientPlayerItem();
        if (playerListItems.Count > Manager.gamePlayers.Count) RemovePlayerItem();
        if (playerListItems.Count == Manager.gamePlayers.Count) UpdatePlayerItem();
    }

    public void FindLocalPlayer() {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = localPlayerObject.GetComponent<PlayerController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerController player in Manager.gamePlayers)
        {
            GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
            PlayerListItem newPlayerListItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerListItemScript.playerName = player.playerName;
            newPlayerListItemScript.connectionID = player.connectionID;
            newPlayerListItemScript.playerSteamID = player.playerSteamID;
            newPlayerListItemScript.isReady = player.isReady;

            newPlayerListItemScript.SetPlayerValues();

            newPlayerListItemScript.transform.SetParent(playerListViewContent.transform);
            newPlayerListItemScript.transform.localScale = Vector3.one;

            playerListItems.Add(newPlayerListItemScript);
        }

        playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerController player in Manager.gamePlayers)
        {
            if (!playerListItems.Any(b => b.connectionID == player.connectionID)) {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerListItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerListItemScript.playerName = player.playerName;
                newPlayerListItemScript.connectionID = player.connectionID;
                newPlayerListItemScript.playerSteamID = player.playerSteamID;
                newPlayerListItemScript.isReady = player.isReady;

                newPlayerListItemScript.SetPlayerValues();

                newPlayerListItemScript.transform.SetParent(playerListViewContent.transform);
                newPlayerListItemScript.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerListItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerController player in Manager.gamePlayers)
        {
            foreach(PlayerListItem playerListItemScript in playerListItems) {
                if (playerListItemScript.connectionID == player.connectionID) {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.isReady = player.isReady;
                    playerListItemScript.SetPlayerValues();

                    if (player == LocalPlayerController) {
                        UpdateButton();
                    }
                }
            }
        }

        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();
        
        foreach (PlayerListItem playerListItem in playerListItems)
        {
            if (!Manager.gamePlayers.Any(b => b.connectionID == playerListItem.connectionID)) {
                playerListItemsToRemove.Add(playerListItem);
            }
        }

        if (playerListItemsToRemove.Count > 0) {
            foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
            {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                playerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    public void StartGame(string sceneName) {
        LocalPlayerController.CanStartGame(sceneName);
    }

}
