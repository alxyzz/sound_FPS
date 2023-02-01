using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class PlayerController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int connectionID;
    [SyncVar] public int playerIDNumber;
    [SyncVar] public ulong playerSteamID;
    [SyncVar] public uint kills;
    [SyncVar] public uint deaths;
    [SyncVar] public uint health;

   

    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool isReady;

    //Cosmetics
    [SyncVar(hook = nameof(SendPlayerColor))] public int PlayerColor;
    [SyncVar(hook = nameof(SendPlayerColor))] public int PlayerFace;

    //Components
    private FPSMovementController fpsController;
    private FPS_UI_Component fpsUI;

    private void PlayerReadyUpdate(bool oldReady, bool newReady)
    {
        if (isServer) {
            this.isReady = newReady;
        }
        if (isClient) {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady() {
        this.PlayerReadyUpdate(this.isReady, !this.isReady);
    }

    void OnChangeHealth(uint oldvalue, uint newvalue)
    {
        fpsUI.health.text = newvalue.ToString();
        
    }






    public void ChangeReady() {
        if (hasAuthority) CmdSetPlayerReady();
    }

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

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
        fpsController = GetComponent<FPSMovementController>();
    }

    #region Health, Death, and Taxes
    public void CmdSuicide()
    {
        CmdTakeDamage(health);
        RefreshHealthUI();
    }

    public void RefreshHealthUI()
    {
        fpsUI.health.text = health.ToString();
    }
    [Command(requiresAuthority = false)]
    public void CmdTakeDamage(uint i)
    {
        
        if (fpsController.isDead) return;

        health -= (uint)i;

        if (health > 0) return;
        Debug.LogError("Player just died. Disregard this error.");
       RpcDie();
    }
    [ClientRpc]
    void RpcDie()
    {
        Debug.Log("Player has died!");
        gameObject.SetActive(false);
        fpsController.SetRandomPosition();
        fpsController.isDead = true;
        StartCoroutine(delayRespawn());
    }

   IEnumerator delayRespawn()
    {
        yield return new WaitForSecondsRealtime(5);
        gameObject.SetActive(true);
    }


    //public void RpcDie()
    //{
    //    Debug.Log("Person just died.");
    //    gameObject.SetActive(false);
    //    StartCoroutine(RespawnDelay());

    //}
    [Command]
    private void CauseDeathServer()
    {
        //ServerPlayerDie(this);
    }

    /// <summary>
    ///     The server side method that handles a player's death
    /// </summary>
    [Server]
    private void ServerPlayerDie(string sourcePlayerId)
    {
        //IsDead = true;

        ////Send a message about this player's death
        //NetworkServer.SendToAll(new PlayerDiedMessage
        //{
        //    PlayerKilled = transform.name,
        //    PlayerKiller = sourcePlayerId,
        //    WeaponName = weaponManager.GetActiveWeapon().weaponId
        //}, 3);

        ////Remove all the weapons on the player
        //weaponManager.RemoveAllWeapons();

        ////Call the client side code on each player
        //RpcClientPlayerDie(sourcePlayerId);

        ////Disable movement
        //playerMovementManager.enabled = false;

        ////Update the stats, for both players
        //PlayerManager killer = GameManager.GetPlayer(sourcePlayerId);
        //Deaths++;
        //if (sourcePlayerId != transform.name)
        //    killer.Kills++;

        //StartCoroutine(ServerPlayerRespawn());
    }


    //public void RpcRespawn()
    //{

    //}

    //public void CmdRespawn()
    //{

    //}
    //[ClientRpc]
    private void RpcClientPlayerDie(string killerPlayer)
    {
        //try
        //{
        //    //Disable the collider, or the Char controller
        //    if (isLocalPlayer)
        //    {
        //        //Disable the HUD
        //        uiManager.SetHud(false);
        //        uiManager.SetDeathScreen(GameManager.GetPlayer(killerPlayer), true);
        //    }

        //    //Disable movement
        //    playerMovementManager.enabled = false;

        //    foreach (GameObject toDisable in disableGameObjectsOnDeath) toDisable.SetActive(false);
        //}
        //catch (Exception ex)
        //{
        //    Logger.Error(ex, "Something went wrong in {MethodName}!", nameof(RpcClientPlayerDie));
        //}
    }
    [ClientRpc]
    private void RpcClientRespawn()
    {
        //try
        //{
        //    //Enable game objects
        //    foreach (GameObject toEnable in disableGameObjectsOnDeath) toEnable.SetActive(true);

        //    //Enable movement
        //    fpsController.isDead = false;

        //    //Enable the collider, or the Char controller
        //    if (isLocalPlayer)
        //    {
        //        //Enable our HUD
        //        uiManager.SetHud(true);
        //        uiManager.SetDeathScreen(null, false);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Logger.Error(ex, "Something went wrong in {MethodName}!", nameof(RpcClientRespawn));
        //}
    }


    public void CmdRespawn()
    {
        fpsController.isDead = false;
    }



    #endregion

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.gamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.gamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName) {
        this.PlayerNameUpdate(this.playerName, playerName);
    }

    private void PlayerNameUpdate(string oldValue, string newValue) {
        if (isServer) {
            this.playerName = newValue;
        } 

        if (isClient) {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string sceneName) {
        if (hasAuthority) {
            CmdCanStartGame(sceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string sceneName) {
        manager.StartGame(sceneName);
    }

    //Cosmetics
    [Command]
    private void CmdUpdatePlayerColor(int newValue)
    {
        SendPlayerColor(PlayerColor, newValue);
    }

    private void SendPlayerColor(int oldValue, int newValue)
    {
        if (isServer) {
            PlayerColor = newValue;
        } 

        if (isClient && (oldValue != newValue)) {
            UpdateColor(newValue);
        }
    }

    private void UpdateFace(int message) {
        PlayerColor = message;
    }


    [Command]
    private void CmdUpdatePlayerFace(int newValue)
    {
        SendPlayerFace(PlayerColor, newValue);
    }

    private void SendPlayerFace(int oldValue, int newValue)
    {
        if (isServer)
        {
            PlayerColor = newValue;
        }

        if (isClient && (oldValue != newValue))
        {
            UpdateColor(newValue);
        }
    }

    private void UpdateColor(int message)
    {
        PlayerColor = message;
    }
}
